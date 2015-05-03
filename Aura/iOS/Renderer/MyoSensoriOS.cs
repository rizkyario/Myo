using System;
using Aura.iOS;
using Myo;
using UIKit;
using Foundation;
using CoreAnimation;
using OpenTK;
using Xamarin.Forms.Platform.iOS;
using Aura.Helpers;
using System.Collections.ObjectModel;
using OxyPlot.Series;
using OxyPlot;
using System.Collections.Generic;
using OxyPlot.Axes;

[assembly: Xamarin.Forms.Dependency (typeof (MyoSensoriOS))]
namespace Aura.iOS
{
	public class MyoSensoriOS: IMyoSensor
	{
		TLMPose currentPose;
		UINavigationController controller;
		TLMMyo myo;

		static volatile MyoSensoriOS _Instance;
		static object _SyncRoot = new Object();

		public static MyoSensoriOS Instance {
			get {
				if (_Instance == null)
					lock (_SyncRoot)
						if (_Instance == null)
							_Instance = new MyoSensoriOS ();

				return _Instance;
			}
		}


		public void Initialize()
		{
//			if (Settings.DefaultMyo != Guid.Empty)
//				TLMHub.SharedHub ().AttachByIdentifier (Settings.DefaultMyo);

			TLMHub.SharedHub ().AttachToAdjacent ();
			TLMHub.SharedHub ().LockingPolicy = TLMLockingPolicy.TLMLockingPolicyNone;
			TLMHub.SharedHub ().ShouldNotifyInBackground = true;

			App.MyoDataStream.Status = "Innitiated";

			TLMHub.Notifications.ObserveTLMHubDidConnectDevice(deviceConnected);
			TLMHub.Notifications.ObserveTLMHubDidDisconnectDevice(deviceDisconnected);


			TLMMyo.Notifications.ObserveTLMMyoDidReceiveEmgEvent(receiveEmgEvent);
			TLMMyo.Notifications.ObserveTLMMyoDidReceiveArmSyncEvent(armSynced);
			TLMMyo.Notifications.ObserveTLMMyoDidReceiveArmUnsyncEvent(armUnsync);
			TLMMyo.Notifications.ObserveTLMMyoDidReceiveLockEvent(lockDevice);
			TLMMyo.Notifications.ObserveTLMMyoDidReceiveUnlockEvent(unlockDevice);
			TLMMyo.Notifications.ObserveTLMMyoDidReceiveOrientationEvent(receiveOrientationEvent);
			TLMMyo.Notifications.ObserveTLMMyoDidReceiveAccelerometerEvent(receiveAccelerometerEvent);
			TLMMyo.Notifications.ObserveTLMMyoDidReceivePoseChanged(receivePoseChanged);
		}

		public void Connect ()
		{
			Xamarin.Insights.Track("touchupinside");

			controller = TLMSettingsViewController.SettingsInNavigationController();
			controller.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
			// Present the settings view controller modally.
			AppDelegate.MainMenu.PresentViewController(controller, true, null);
		}

		void deviceConnected(object sender, NSNotificationEventArgs e)
		{
			Xamarin.Insights.Track("deviceConnected");

			myo = TLMHub.SharedHub ().GetDevices () [0]as TLMMyo;

			myo.SetStreamEmg (TLMStreamEmgType.TLMStreamEmgEnabled);
			Settings.DefaultMyo = myo.Identifier;

			App.MyoDataStream.Status = "Connected";
		}

		void deviceDisconnected(object sender, NSNotificationEventArgs e)
		{
			Xamarin.Insights.Track("deviceDisconnected");

			var notification = e.Notification;
			App.MyoDataStream.Status = "Disconnected";
		}

		void armSynced(object sender, NSNotificationEventArgs e)
		{
			Xamarin.Insights.Track("armSynced");

			var notification = e.Notification;
			// Retrieve the arm event from the notification's userInfo with the kTLMKeyArmSyncEvent key.
			TLMArmSyncEvent armEvent = notification.UserInfo[TLMMyo.TLMKeyArmSyncEvent] as TLMArmSyncEvent;

			// Update the armLabel with arm information
			if (armEvent != null) {
				String armString = armEvent.Arm == TLMArm.TLMArmRight ? "Right" : "Left";
				String directionString = armEvent.xDirection == TLMArmXDirection.TLMArmXDirectionTowardWrist ? "Toward Wrist" : "Toward Elbow";
				App.MyoDataStream.Status = string.Format ("Sync Arm: {0} X-Direction: {1}", armString, directionString);

			}
		}

		void armUnsync (object sender, NSNotificationEventArgs e)
		{
			Xamarin.Insights.Track("armLost");

			var notification = e.Notification;
			App.MyoDataStream.Status = "Unsync";

		}

		void lockDevice (object sender, NSNotificationEventArgs e)
		{
			Xamarin.Insights.Track("lockDevice");

			App.MyoDataStream.Status = "Locked";
		}

		void unlockDevice (object sender, NSNotificationEventArgs e)
		{
			Xamarin.Insights.Track("unlockDevice");

			App.MyoDataStream.Status = "Unlock";
		}

		void receiveOrientationEvent(object sender, NSNotificationEventArgs e)
		{
			Xamarin.Insights.Track("receiveOrientationEvent");

			var notification = e.Notification;
			// Retrieve the orientation from the NSNotification's userInfo with the kTLMKeyOrientationEvent key.
			TLMOrientationEvent orientationEvent = notification.UserInfo[TLMMyo.TLMKeyOrientationEvent] as TLMOrientationEvent;

			// Create Euler angles from the quaternion of the orientation.
			TLMEulerAngles angles = TLMEulerAngles.GetAnglesWithQuaternion(orientationEvent.Quaternion);

			try {
				// Next, we want to apply a rotation and perspective transformation based on the pitch, yaw, and roll.
				var pitchF = Convert.ToSingle(angles.Pitch.Radians);
				var yawF = Convert.ToSingle(angles.Yaw.Radians);
				var rollF = Convert.ToSingle(angles.Roll.Radians);

				var pitch = CATransform3D.MakeRotation(pitchF, -1.0f, 0.0f, 0.0f);
				var yaw = CATransform3D.MakeRotation(yawF, 0.0f, 1.0f, 0.0f);
				var roll = CATransform3D.MakeRotation(rollF, 0.0f, 0.0f, -1.0f);

				CATransform3D rotationAndPerspectiveTransform = pitch.Concat(yaw).Concat(roll);

				// Apply the rotation and perspective transform to helloLabel.
				//				helloLabel.Layer.Transform = rotationAndPerspectiveTransform;

				App.MyoDataStream.Orientation = rotationAndPerspectiveTransform.ToString();
			} catch(Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex.Message);
				Xamarin.Insights.Report(ex);
			}
		}

		void receiveAccelerometerEvent(object sender, NSNotificationEventArgs e)
		{
			Xamarin.Insights.Track("receiveAccelerometerEvent");

			var notification = e.Notification;
			// Retrieve the accelerometer event from the NSNotification's userInfo with the kTLMKeyAccelerometerEvent.
			TLMAccelerometerEvent accelerometerEvent = notification.UserInfo[TLMMyo.TLMKeyAccelerometerEvent] as TLMAccelerometerEvent;

			// Get the acceleration vector from the accelerometer event.
			Vector3 accelerationVector = accelerometerEvent.Vector;
			// Calculate the magnitude of the acceleration vector.
			float magnitude = accelerationVector.Length;
//			App.MyoDataStream.Accelaration = new XLabs.Vector3 (accelerationVector.X,accelerationVector.Y,accelerationVector.Z);

			int Counter = (((LineSeries)App.MyoDataStream.AccelarationModel.Series [0]).ItemsSource as List<DataPoint>).Count;
			int limit =(int) ((LinearAxis)App.MyoDataStream.AccelarationModel.Axes [0]).Maximum;
				
			if (Counter == limit) {
				for (int i = 0; i < 3; i++)
					(((LineSeries)App.MyoDataStream.AccelarationModel.Series [i]).ItemsSource as List<DataPoint>).Clear ();
				Counter = 0;
			}

			(((LineSeries)App.MyoDataStream.AccelarationModel.Series [0]).ItemsSource as List<DataPoint>).Add (new DataPoint (Counter, accelerationVector.X));
			(((LineSeries)App.MyoDataStream.AccelarationModel.Series [1]).ItemsSource as List<DataPoint>).Add (new DataPoint (Counter, accelerationVector.Y));
			(((LineSeries)App.MyoDataStream.AccelarationModel.Series [2]).ItemsSource as List<DataPoint>).Add (new DataPoint (Counter, accelerationVector.Z));

			App.MyoDataStream.AccelarationModel.InvalidatePlot (true);

			// Update the progress bar based on the magnitude of the acceleration vector.
//			accelerationProgressBar.Progress = magnitude / 8;

			//			accelerationLabel.Text = string.Format("x: {0}, y: {1}, z: {2}", 
			//				accelerationVector.X, accelerationVector.Y, accelerationVector.Z);
		}

		void receiveEmgEvent(object sender, NSNotificationEventArgs e)
		{
			Xamarin.Insights.Track("receiveEmgEvent");
			var notification = e.Notification;

			// Retrieve the emg event from the NSNotification's userInfo with the TLMKeyEMGEvent.
			TLMEmgEvent emgEvent = notification.UserInfo [TLMMyo.TLMKeyEMGEvent] as TLMEmgEvent;

			int Counter = (((LineSeries)App.MyoDataStream.EMGModel.Series [0]).ItemsSource as List<DataPoint>).Count;
			int limit =(int) ((LinearAxis)App.MyoDataStream.EMGModel.Axes [0]).Maximum;

			if (Counter == limit) {
				for (int i = 0; i < 8; i++)
					(((LineSeries)App.MyoDataStream.EMGModel.Series [i]).ItemsSource as List<DataPoint>).Clear ();

				Counter = 0;
			}

			for (int i = 0; i < 8; i++)
				(((LineSeries)App.MyoDataStream.EMGModel.Series [i]).ItemsSource as List<DataPoint>).Add (new DataPoint (Counter, (Double.Parse(NSNumber.FromObject (new NSObject (emgEvent.RawData.ValueAt ((nuint)i))).ToString()))/128*2.5));

			App.MyoDataStream.EMGModel.InvalidatePlot (true);

			App.MyoDataStream.EMG = emgEvent.RawData.ToString();
		}

		void receivePoseChanged(object sender, NSNotificationEventArgs e)
		{
			Xamarin.Insights.Track("receivePoseChanged");

			var notification = e.Notification;
			// Retrieve the pose from the NSNotification's userInfo with the kTLMKeyPose key.
			TLMPose pose = notification.UserInfo[TLMMyo.TLMKeyPose] as TLMPose;
			currentPose = pose;

			// Handle the cases of the TLMPoseType enumeration, and change the color of helloLabel based on the pose we receive.
			switch(pose.Type) {
			case TLMPoseType.TLMPoseTypeUnknown:
				App.MyoDataStream.Pose = @"Unknown";
				break;
			case TLMPoseType.TLMPoseTypeRest:
				App.MyoDataStream.Pose = @"";
				break;
			case TLMPoseType.TLMPoseTypeFist:
				App.MyoDataStream.Pose = @"Fist";
				break;
			case TLMPoseType.TLMPoseTypeWaveIn:
				App.MyoDataStream.Pose = @"Wave In";
				break;
			case TLMPoseType.TLMPoseTypeWaveOut:
				App.MyoDataStream.Pose = @"Wave Out";
				break;
			case TLMPoseType.TLMPoseTypeFingersSpread:
				App.MyoDataStream.Pose = @"Fingers Spread";
				break;
			case TLMPoseType.TLMPoseTypeDoubleTap:
				App.MyoDataStream.Pose = @"Double Tap";
				break;
			}
		}
	}
}

