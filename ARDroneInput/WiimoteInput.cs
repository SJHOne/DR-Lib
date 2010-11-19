﻿using System;
using System.Collections.Generic;
using System.Text;
using WiimoteLib;

namespace ARDrone.Input
{
    public class WiimoteInput : GenericInput
    {
        enum Axis
        {
            // "Normal" axes
            Axis_X, Axis_Y, Axis_Z,
            // Nunchuk axis
            Axis_Nunchuk_X, Axis_Nunchuk_Y
        }

        enum Button {
            // "Normal" buttons
            Button_A, Button_B, Button_Minus, Button_Plus, Button_Home, Button_One, Button_Two, Button_Left, Button_Up, Button_Right, Button_Down,
            // Nunchuk buttons
            Button_Nunchuk_C,
            // Classic controller buttons
            Button_Classic_A, Button_Classic_B, Button_Classic_X, Button_Classic_Y, Button_Classic_L, Button_Classic_R,
            Button_Classic_Minus, Button_Classic_Plus, Button_Classic_Home,
            Button_Classic_Left, Button_Classic_Up, Button_Classic_Right, Button_Classic_Down
        };

        Dictionary<String, float> currentAxisValues = new Dictionary<String, float>();
        List<String> currentButtonsPressed = new List<String>();

        Wiimote wiimote = null;

        public WiimoteInput(Wiimote wiimote) : base()
        {
            wiimote.WiimoteChanged += wiimote_WiimoteChanged;
            wiimote.WiimoteExtensionChanged += wiimote_WiimoteExtensionChanged;

            wiimote.Connect();

            if (wiimote.WiimoteState.ExtensionType != ExtensionType.BalanceBoard)
            {
                wiimote.SetReportType(InputReport.IRExtensionAccel, IRSensitivity.Maximum, true);
            }

            this.wiimote = wiimote;

            List<String> validAxes = new List<String>();
            foreach (Axis axis in Enum.GetValues(typeof(Axis)))
            {
                validAxes.Add(axis.ToString());
            }

            List<String> validButtons = new List<String>();
            foreach (Button button in Enum.GetValues(typeof(Button)))
            {
                validButtons.Add(button.ToString());
            }

            CreateMapping(validButtons, validAxes);
        }

        protected override void CreateStandardMapping()
        {
            mapping.SetAxisMappings(Axis.Axis_Y, Axis.Axis_X, "Button_Left-Button_Right", "Button_B-Button_A");
            mapping.SetButtonMappings("", Button.Button_Plus, Button.Button_Plus, Button.Button_Minus, Button.Button_Home, "");
        }

        public override void Dispose()
        {
            wiimote.Disconnect();
        }

        public override List<String> GetPressedButtons()
        {
            return currentButtonsPressed;
        }

        public override Dictionary<String, float> GetAxisValues()
        {
            return currentAxisValues;
        }

        public void SetLEDs(bool led1, bool led2, bool led3, bool led4)
        {
            wiimote.SetLEDs(led1, led2, led3, led4);
        }

        public void SetRumble(bool on)
        {
            wiimote.SetRumble(on);
        }

        void wiimote_WiimoteChanged(object sender, WiimoteChangedEventArgs e)
		{
            WiimoteState state = e.WiimoteState;

            Dictionary<String, float> axisValues = new Dictionary<String, float>();

            axisValues[Axis.Axis_X.ToString()] = state.AccelState.Values.X;
            axisValues[Axis.Axis_Y.ToString()] = state.AccelState.Values.Y;
            axisValues[Axis.Axis_Z.ToString()] = state.AccelState.Values.Z;

            axisValues[Axis.Axis_Nunchuk_X.ToString()] = state.NunchukState.Joystick.X;
            axisValues[Axis.Axis_Nunchuk_Y.ToString()] = state.NunchukState.Joystick.Y;

            List<String> buttonsPressed = new List<String>();

            if (state.ButtonState.A) { buttonsPressed.Add(Button.Button_A.ToString()); }
            if (state.ButtonState.B) { buttonsPressed.Add(Button.Button_B.ToString()); }
            if (state.ButtonState.One) { buttonsPressed.Add(Button.Button_One.ToString()); }
            if (state.ButtonState.Two) { buttonsPressed.Add(Button.Button_Two.ToString()); }
            if (state.ButtonState.Minus) { buttonsPressed.Add(Button.Button_Minus.ToString()); }
            if (state.ButtonState.Plus) { buttonsPressed.Add(Button.Button_Plus.ToString()); }
            if (state.ButtonState.Home) { buttonsPressed.Add(Button.Button_Home.ToString()); }

            if (state.NunchukState.C ) { buttonsPressed.Add(Button.Button_Nunchuk_C.ToString()); }

            if (state.ClassicControllerState.ButtonState.A) { buttonsPressed.Add(Button.Button_Classic_A.ToString()); }
            if (state.ClassicControllerState.ButtonState.B) { buttonsPressed.Add(Button.Button_Classic_B.ToString()); }
            if (state.ClassicControllerState.ButtonState.X) { buttonsPressed.Add(Button.Button_Classic_X.ToString()); }
            if (state.ClassicControllerState.ButtonState.Y) { buttonsPressed.Add(Button.Button_Classic_Y.ToString()); }
            if (state.ClassicControllerState.ButtonState.ZL) { buttonsPressed.Add(Button.Button_Classic_L.ToString()); }
            if (state.ClassicControllerState.ButtonState.ZR) { buttonsPressed.Add(Button.Button_Classic_R.ToString()); }
            if (state.ClassicControllerState.ButtonState.Left) { buttonsPressed.Add(Button.Button_Classic_Left.ToString()); }
            if (state.ClassicControllerState.ButtonState.Up) { buttonsPressed.Add(Button.Button_Classic_Up.ToString()); }
            if (state.ClassicControllerState.ButtonState.Right) { buttonsPressed.Add(Button.Button_Classic_Right.ToString()); }
            if (state.ClassicControllerState.ButtonState.Down) { buttonsPressed.Add(Button.Button_Classic_Down.ToString()); }
            if (state.ClassicControllerState.ButtonState.Minus) { buttonsPressed.Add(Button.Button_Classic_Minus.ToString()); }
            if (state.ClassicControllerState.ButtonState.Plus) { buttonsPressed.Add(Button.Button_Classic_Plus.ToString()); }
            if (state.ClassicControllerState.ButtonState.Home) { buttonsPressed.Add(Button.Button_Classic_Home.ToString()); }

            currentAxisValues = axisValues;
            currentButtonsPressed = buttonsPressed;
		}

        void wiimote_WiimoteExtensionChanged(object sender, WiimoteExtensionChangedEventArgs e)
        {
            // Nothing to do (for now)
        }

        public override bool IsDevicePresent
        {
            get
            {
                // TODO
                return true;
            }
        }

        public override String DeviceName
        {
            get
            {
                if (wiimote == null) { return string.Empty; }
                else { return wiimote.HIDDevicePath; }
            }
        }

        public override String FilePrefix
        {
            get
            {
                if (wiimote == null) { return string.Empty; }
                else { return "WM"; }
            }
        }

        public override String DeviceInstanceId
        {
            get
            {
                if (wiimote == null) { return string.Empty; }
                else { return wiimote.ID.ToString(); }
            }
        }
    }
}
