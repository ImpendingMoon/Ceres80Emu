namespace Ceres80Emu
{
    internal class InputManager
    {
        public InputManager()
        {
            SetDefaultKeyMappings();
        }

        public byte GetButtonStateByte()
        {
            byte result = 0;

            for(int i = 0; i < 8; i++)
            {
                // Buttons are pulled high and shorted to ground when pressed
                byte value = (byte)(_pressedButtons[i] ? 0 : 1);
                result |= (byte)(value << i);
            }

            return result;
        }

        public void HandleKeyDown(Keys key)
        {
            if(_keyMap.TryGetValue(key, out InputButton button))
            {
                _pressedButtons[(int) button] = true;
            }
        }

        public void HandleKeyUp(Keys key)
        {
            if (_keyMap.TryGetValue(key, out InputButton button))
            {
                _pressedButtons[(int)button] = false;
            }
        }

        public void SetDefaultKeyMappings()
        {
            // Don't want to keep old mappings
            ClearKeyMappings();

            _keyMap[Keys.Left] = InputButton.Left;
            _keyMap[Keys.Right] = InputButton.Right;
            _keyMap[Keys.Up] = InputButton.Up;
            _keyMap[Keys.Down] = InputButton.Down;
            _keyMap[Keys.Z] = InputButton.A;
            _keyMap[Keys.X] = InputButton.B;
            _keyMap[Keys.Enter] = InputButton.Start;
            _keyMap[Keys.RShiftKey] = InputButton.Select;
        }

        public void SetKeyMapping(Keys key, InputButton button)
        {
            _keyMap[key] = button;
        }

        public void RemoveKeyMapping(Keys key)
        {
            _keyMap.Remove(key);
        }

        public void ClearKeyMappings()
        {
            _keyMap.Clear();
        }

        private Dictionary<Keys, InputButton> _keyMap = [];

        // Indexed by InputButton enum
        private bool[] _pressedButtons = new bool[8];
    }

    internal enum InputButton
    {
        Left = 0,
        Right,
        Up,
        Down,
        A,
        B,
        Start,
        Select
    }
}
