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
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

[assembly: Xamarin.Forms.Dependency (typeof (MyoSensoriOS))]
namespace Aura.iOS
{
	public class MyoSensoriOS: IMyoSensor
	{
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

		public void AttachToAdjacent()
		{
			TLMHub.SharedHub ().AttachToAdjacent ();
		}

		public void Initialize()
		{
//			if (Settings.DefaultMyo != Guid.Empty)
//				TLMHub.SharedHub ().AttachByIdentifier (Settings.DefaultMyo);

			TLMHub.SharedHub ().AttachToAdjacent ();
			TLMHub.SharedHub ().LockingPolicy = TLMLockingPolicy.TLMLockingPolicyNone;
			TLMHub.SharedHub ().ShouldNotifyInBackground = true;

			App.AuraVM.Status = "Innitiated";

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

			App.AuraVM.Status = "Connected";
		}

		void deviceDisconnected(object sender, NSNotificationEventArgs e)
		{
			Xamarin.Insights.Track("deviceDisconnected");

			var notification = e.Notification;
			App.AuraVM.Status = "Disconnected";
			App.AuraVM.IsSynced = false;

		}

		void armSynced(object sender, NSNotificationEventArgs e)
		{
			Xamarin.Insights.Track("armSynced");
			App.AuraVM.IsSynced = true;
			var notification = e.Notification;
			// Retrieve the arm event from the notification's userInfo with the kTLMKeyArmSyncEvent key.
			TLMArmSyncEvent armEvent = notification.UserInfo[TLMMyo.TLMKeyArmSyncEvent] as TLMArmSyncEvent;

			// Update the armLabel with arm information
			if (armEvent != null) {
				String armString = armEvent.Arm == TLMArm.TLMArmRight ? "Right" : "Left";
				String directionString = armEvent.xDirection == TLMArmXDirection.TLMArmXDirectionTowardWrist ? "Toward Wrist" : "Toward Elbow";
				App.AuraVM.Status = string.Format ("Sync Arm: {0} X-Direction: {1}", armString, directionString);

			}
		}

		void armUnsync (object sender, NSNotificationEventArgs e)
		{
			Xamarin.Insights.Track("armLost");

			var notification = e.Notification;
			App.AuraVM.Status = "Unsync";
			App.AuraVM.IsSynced = false;

		}

		void lockDevice (object sender, NSNotificationEventArgs e)
		{
			Xamarin.Insights.Track("lockDevice");

			App.AuraVM.Status = "Locked";
		}

		void unlockDevice (object sender, NSNotificationEventArgs e)
		{
			Xamarin.Insights.Track("unlockDevice");

			App.AuraVM.Status = "Unlock";
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
			DateTime dt = accelerometerEvent.Timestamp.ToDateTime();

			// Calculate the magnitude of the acceleration vector.
			float magnitude = accelerationVector.Length;
//			App.MyoDataStream.Accelaration = new XLabs.Vector3 (accelerationVector.X,accelerationVector.Y,accelerationVector.Z);



//			if (RCounter > ((LinearAxis)App.AuraVM.RecordedAccelarationModel.Axes [0]).Maximum)
//				((LinearAxis)App.AuraVM.AccelarationModel.Axes [0]).Maximum = RCounter;

			if (App.AuraVM.IsAutomatic) {
				if (accelerationVector.X > accelerationVector.Z && accelerationVector.Z > accelerationVector.Y) {
					if (App.AuraVM.IsRecording) {
						App.AuraVM.StopGesture ();
						if (!App.AuraVM.IsTraining) {
							if (!App.AuraVM.IsRecognizing) {
								App.AuraVM.IsRecognizing = true;
//								App.AuraVM.GesturesVM.RecognizeGesture (App.AuraVM.AccelarationModel);
							}
						} else {
//							App.AuraVM.GesturesVM.RecognizeGesture (App.AuraVM.RecordedAccelarationModel);
						}
					}
				} else {
					if (App.AuraVM.IsNotRecording)
						App.AuraVM.RecordGesture ();
				}
			}



			if (App.AuraVM.IsRecording) {
				if (App.AuraVM.IsTraining) {
					int RCounter = (((LineSeries)App.AuraVM.RecordedAccelarationModel.Series [0]).ItemsSource as List<DataPoint>).Count;

					(((LineSeries)App.AuraVM.RecordedAccelarationModel.Series [0]).ItemsSource as List<DataPoint>).Add (new DataPoint (RCounter, accelerationVector.X));
					(((LineSeries)App.AuraVM.RecordedAccelarationModel.Series [1]).ItemsSource as List<DataPoint>).Add (new DataPoint (RCounter, accelerationVector.Y));
					(((LineSeries)App.AuraVM.RecordedAccelarationModel.Series [2]).ItemsSource as List<DataPoint>).Add (new DataPoint (RCounter, accelerationVector.Z));
					App.AuraVM.RecordedAccelarationModel.InvalidatePlot (true);
				} else {
					int Counter = (((LineSeries)App.AuraVM.AccelarationModel.Series [0]).ItemsSource as List<DataPoint>).Count;
					int limit = (int)((LinearAxis)App.AuraVM.AccelarationModel.Axes [0]).Maximum;

					(((LineSeries)App.AuraVM.AccelarationModel.Series [0]).ItemsSource as List<DataPoint>).Add (new DataPoint (Counter, accelerationVector.X));
					(((LineSeries)App.AuraVM.AccelarationModel.Series [1]).ItemsSource as List<DataPoint>).Add (new DataPoint (Counter, accelerationVector.Y));
					(((LineSeries)App.AuraVM.AccelarationModel.Series [2]).ItemsSource as List<DataPoint>).Add (new DataPoint (Counter, accelerationVector.Z));

					if (Counter == limit) {
						for (int i = 0; i < 3; i++)
							(((LineSeries)App.AuraVM.AccelarationModel.Series [i]).ItemsSource as List<DataPoint>).Clear ();
						Counter = 0;
					}
					App.AuraVM.AccelarationModel.InvalidatePlot (true);
				}
				App.AuraVM.Poses.Add (App.AuraVM.CurrentPose);
			}
			


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



			if (App.AuraVM.IsRecording) {
				int RCounter = (((LineSeries)App.AuraVM.RecordedEMGModel.Series [0]).ItemsSource as List<DataPoint>).Count;

				for (int i = 0; i < 8; i++)
					(((LineSeries)App.AuraVM.RecordedEMGModel.Series [i]).ItemsSource as List<DataPoint>).Add (new DataPoint (RCounter, (Double.Parse(NSNumber.FromObject (new NSObject (emgEvent.RawData.ValueAt ((nuint)i))).ToString()))/128*2.5));
				
				App.AuraVM.RecordedEMGModel.InvalidatePlot (true);

			} else {
				int Counter = (((LineSeries)App.AuraVM.EMGModel.Series [0]).ItemsSource as List<DataPoint>).Count;
				int limit =(int) ((LinearAxis)App.AuraVM.EMGModel.Axes [0]).Maximum;

				if (Counter == limit) {
					for (int i = 0; i < 8; i++)
						(((LineSeries)App.AuraVM.EMGModel.Series [i]).ItemsSource as List<DataPoint>).Clear ();

					Counter = 0;
				}

				for (int i = 0; i < 8; i++)
					(((LineSeries)App.AuraVM.EMGModel.Series [i]).ItemsSource as List<DataPoint>).Add (new DataPoint (Counter, (Double.Parse(NSNumber.FromObject (new NSObject (emgEvent.RawData.ValueAt ((nuint)i))).ToString()))/128*2.5));

				App.AuraVM.EMGModel.InvalidatePlot (true);


			}
		}

		void receivePoseChanged(object sender, NSNotificationEventArgs e)
		{
			Xamarin.Insights.Track("receivePoseChanged");

			var notification = e.Notification;
			// Retrieve the pose from the NSNotification's userInfo with the kTLMKeyPose key.
			TLMPose pose = notification.UserInfo[TLMMyo.TLMKeyPose] as TLMPose;

			// Handle the cases of the TLMPoseType enumeration, and change the color of helloLabel based on the pose we receive.
			switch(pose.Type) {
			case TLMPoseType.TLMPoseTypeUnknown:
				App.AuraVM.CurrentPose = 0;
				break;
			case TLMPoseType.TLMPoseTypeRest:
				App.AuraVM.CurrentPose = 1;
								break;
			case TLMPoseType.TLMPoseTypeFist:
				App.AuraVM.CurrentPose = 2;
								break;
			case TLMPoseType.TLMPoseTypeWaveIn:
				App.AuraVM.CurrentPose = 3;
								break;
			case TLMPoseType.TLMPoseTypeWaveOut:
				App.AuraVM.CurrentPose = 4;
								break;
			case TLMPoseType.TLMPoseTypeFingersSpread:
				App.AuraVM.CurrentPose = 5;
								break;
			case TLMPoseType.TLMPoseTypeDoubleTap:
				App.AuraVM.CurrentPose = 6;
								break;
			}


		}
	}
}

