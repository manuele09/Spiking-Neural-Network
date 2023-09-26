using System;
using System.Collections.Generic;


namespace SLN
{
    /// <summary>
    /// A complete input to the network, i.e. all the four features plus the reward
    /// </summary>
    [Serializable]
    public class NetworkInput
    {
        public int id;
        public int[] winner_ids = new int[4];
        public int[] winner_motors = new int[4];


        private int _color;

        /// <summary>
        /// The <b>COLOR</b> feature
        /// </summary>
        internal int COLOR
        {
            get { return _color; }
        }

        private int _size;

        /// <summary>
        /// The <b>SIZE</b> feature
        /// </summary>
        internal int SIZE
        {
            get { return _size; }
        }

        private int _hdistr;

        /// <summary>
        /// The <b>HORIZONTAL DISTRIBUTEDNESS</b> feature
        /// </summary>
        internal int HDISTR
        {
            get { return _hdistr; }
        }

        private int _vdistr;

        /// <summary>
        /// The <b>VERTICAL DISTRIBUTEDNESS</b> feature
        /// </summary>
        internal int VDISTR
        {
            get { return _vdistr; }
        }

        private bool _reward;

        /// <summary>
        /// The reward condition
        /// </summary>
        internal bool REWARD
        {
            get { return _reward; }
        }

        private bool _end;

        /// <summary>
        /// The end sequence condition
        /// </summary>
        internal bool END
        {
            get { return _end; }
        }

        private int _rewardLevel;

        /// <summary>
        /// The level of the reward
        /// </summary>
        internal int REWARDLEVEL
        {
            get { return _rewardLevel; }
        }

        private double _colorValue;
        /// <summary>
        /// The level associated to the first characteristic (color) in %
        /// </summary>
        internal double COLORVALUE
        {
            get { return _colorValue; }
        }

        private double _sizeValue;
        /// <summary>
        /// The level associated to the second characteristic (size) in %
        /// </summary>
        internal double SIZEVALUE
        {
            get { return _sizeValue; }
        }

        private double _hdistrValue;
        /// <summary>
        /// The level associated to the third characteristic (horizontal distributeness) in %
        /// </summary>
        internal double HDISTRVALUE
        {
            get { return _hdistrValue; }
        }

        private double _vdistrValue;
        /// <summary>
        /// The level associated to the fourth characteristic (vertical distributeness) in %
        /// </summary>
        internal double VDISTRVALUE
        {
            get { return _vdistrValue; }
        }

        private int _motor;

        /// <summary>
        /// The <b>motor condition</b> 
        /// </summary>
        internal int MOTOR
        {
            get { return _motor; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="color">The value of the <b>COLOR</b> feature</param>
        /// <param name="size">The value of the <b>SIZE</b> feature</param>
        /// <param name="hdistr">The value of the <b>HORIZONTAL DISTRIBUTEDNESS</b> feature</param>
        /// <param name="vdistr">The value of the <b>VERTICAL DISTRIBUTEDNESS</b> feature</param>
        /// <param name="reward"><i>true</i> if there's a reward associated to this input, <i>false</i> otherwise</param>
        /// <param name="end"><i>true</i> if this is the end sequence input <i>false</i> otherwise</param>
        /// <param name="level"><i>Positive number, not null</i> The level of the reward </param>
        public NetworkInput(int color, int size, int hdistr, int vdistr, bool reward, bool end, int level, double vcolor, double vsize, double vhdistr, double vvdistr, int motor)
        {
            _color = color;
            _size = size;
            _hdistr = hdistr;
            _vdistr = vdistr;
            _reward = reward;
            _end = end;
            _motor = motor;
            if (level < 1)
                _rewardLevel = 1;
            else
                _rewardLevel = level;

            if (vcolor == 0)
                _colorValue = Constants.DEFAULT_CURRENT_LIQUID;
            else
                _colorValue = Constants.DEFAULT_CURRENT_LIQUID + (vcolor / 100) * Constants.DEFAULT_CURRENT_LIQUID;

            if (vsize == 0)
                _sizeValue = Constants.DEFAULT_CURRENT_LIQUID;
            else
                _sizeValue = Constants.DEFAULT_CURRENT_LIQUID + (vsize / 100) * Constants.DEFAULT_CURRENT_LIQUID;

            if (vhdistr == 0)
                _hdistrValue = Constants.DEFAULT_CURRENT_LIQUID;
            else
                _hdistrValue = Constants.DEFAULT_CURRENT_LIQUID + (vhdistr / 100) * Constants.DEFAULT_CURRENT_LIQUID;

            if (vvdistr == 0)
                _vdistrValue = Constants.DEFAULT_CURRENT_LIQUID;
            else
                _vdistrValue = Constants.DEFAULT_CURRENT_LIQUID + (vvdistr / 100) * Constants.DEFAULT_CURRENT_LIQUID;


        }
        //livello end reward minimo = 1
        public NetworkInput(int color, int size, int hdistr, int vdistr, int motor, int end)
        {
            _color = color;
            _size = size;
            _hdistr = hdistr;
            _vdistr = vdistr;

            if (end >= 1)
                _end = true;
            else
                _end = false;


            _rewardLevel = end;

            if (motor >= 0)
                _motor = motor;
            else
                _motor = -1;

            _colorValue = Constants.DEFAULT_CURRENT_LIQUID;

            _sizeValue = Constants.DEFAULT_CURRENT_LIQUID;

            _hdistrValue = Constants.DEFAULT_CURRENT_LIQUID;

            _vdistrValue = Constants.DEFAULT_CURRENT_LIQUID;

        }



        public NetworkInput(int id, int motor, int end)
                : this(ObjectDetection.FromIdToFeatures(id)[0],
           ObjectDetection.FromIdToFeatures(id)[1],
           -1, -1, motor, end)
        {
            this.id = id;
        }

        public static List<NetworkInput> CreateInputList(int[] ids, int[] motors, int end_reward)
        {
            List<NetworkInput> inputs = new List<NetworkInput>();

            for (int i = 0; i < ids.Length; i++) 
                inputs.Add(new NetworkInput(ids[i], motors[i], i == (ids.Length-1) ? end_reward : -1));

            return inputs;
        }

        /// <summary>
        /// Constructor (reward off by default)
        /// </summary>
        /// <param name="color">The value of the <b>COLOR</b> feature</param>
        /// <param name="size">The value of the <b>SIZE</b> feature</param>
        /// <param name="hdistr">The value of the <b>HORIZONTAL DISTRIBUTEDNESS</b> feature</param>
        /// <param name="vdistr">The value of the <b>VERTICAL DISTRIBUTEDNESS</b> feature</param>
        public NetworkInput(int color, int size, int hdistr, int vdistr)
            : this(color, size, hdistr, vdistr, false, false, 1, 0, 0, 0, 0, -1)
        { }


        /// <summary>
        /// Constructor with reward and end sequence
        /// </summary>
        /// <param name="color">The value of the <b>COLOR</b> feature</param>
        /// <param name="size">The value of the <b>SIZE</b> feature</param>
        /// <param name="hdistr">The value of the <b>HORIZONTAL DISTRIBUTEDNESS</b> feature</param>
        /// <param name="vdistr">The value of the <b>VERTICAL DISTRIBUTEDNESS</b> feature</param>
        /// <param name="reward"><i>true</i> if there's a reward associated to this input, <i>false</i> otherwise</param>
        /// <param name="end"><i>true</i> if this is the end sequence input <i>false</i> otherwise</param>
        public NetworkInput(int color, int size, int hdistr, int vdistr, bool reward, bool end)
            : this(color, size, hdistr, vdistr, reward, end, 1, 0, 0, 0, 0, -1)
        { }

        /// <summary>
        /// Constructor with reward and end sequence
        /// </summary>
        /// <param name="color">The value of the <b>COLOR</b> feature</param>
        /// <param name="size">The value of the <b>SIZE</b> feature</param>
        /// <param name="hdistr">The value of the <b>HORIZONTAL DISTRIBUTEDNESS</b> feature</param>
        /// <param name="vdistr">The value of the <b>VERTICAL DISTRIBUTEDNESS</b> feature</param>
        /// <param name="reward"><i>true</i> if there's a reward associated to this input, <i>false</i> otherwise</param>
        /// <param name="end"><i>true</i> if this is the end sequence input <i>false</i> otherwise</param>
        public NetworkInput(int color, int size, int hdistr, int vdistr, int motor, bool end)
            : this(color, size, hdistr, vdistr, false, end, 1, 0, 0, 0, 0, motor)
        { }

        /// <summary>
        /// Constructor with reward and end sequence
        /// </summary>
        /// <param name="color">The value of the <b>COLOR</b> feature</param>
        /// <param name="size">The value of the <b>SIZE</b> feature</param>
        /// <param name="hdistr">The value of the <b>HORIZONTAL DISTRIBUTEDNESS</b> feature</param>
        /// <param name="vdistr">The value of the <b>VERTICAL DISTRIBUTEDNESS</b> feature</param>
        /// <param name="reward"><i>true</i> if there's a reward associated to this input, <i>false</i> otherwise</param>
        /// <param name="end"><i>true</i> if this is the end sequence input <i>false</i> otherwise</param>
        public NetworkInput(int color, int size, int hdistr, int vdistr, bool reward, bool end, int level)
            : this(color, size, hdistr, vdistr, reward, end, level, 0, 0, 0, 0, -1)
        { }

        public NetworkInput(int color, int size, int hdistr, int vdistr, int motor, bool reward, bool end, int level)
            : this(color, size, hdistr, vdistr, reward, end, level, 0, 0, 0, 0, motor)
        { }
        /// <summary>
        /// Constructor with reward and end sequence
        /// </summary>
        /// <param name="color">The value of the <b>COLOR</b> feature</param>
        /// <param name="size">The value of the <b>SIZE</b> feature</param>
        /// <param name="hdistr">The value of the <b>HORIZONTAL DISTRIBUTEDNESS</b> feature</param>
        /// <param name="vdistr">The value of the <b>VERTICAL DISTRIBUTEDNESS</b> feature</param>
        /// <param name="motor"><i>true</i> if there's a reward associated to this input, <i>false</i> otherwise</param>
        /// <param name="end"><i>true</i> if this is the end sequence input <i>false</i> otherwise</param>
        public NetworkInput(int color, int size, int hdistr, int vdistr, int motor, bool end, int level)
            : this(color, size, hdistr, vdistr, false, end, level, 0, 0, 0, 0, motor)
        { }

        /// <summary>
        /// Constructor with reward and end sequence
        /// </summary>
        /// <param name="color">The value of the <b>COLOR</b> feature</param>
        /// <param name="size">The value of the <b>SIZE</b> feature</param>
        /// <param name="hdistr">The value of the <b>HORIZONTAL DISTRIBUTEDNESS</b> feature</param>
        /// <param name="vdistr">The value of the <b>VERTICAL DISTRIBUTEDNESS</b> feature</param>
        /// <param name="reward"><i>true</i> if there's a reward associated to this input, <i>false</i> otherwise</param>
        /// <param name="end"><i>true</i> if this is the end sequence input <i>false</i> otherwise</param>
        public NetworkInput(int color, int size, int hdistr, int vdistr, bool reward, bool end, double vcolor, double vsize, double vhdistr, double vvdistr)
            : this(color, size, hdistr, vdistr, reward, end, 1, vcolor, vsize, vhdistr, vvdistr, -1)
        { }

        /// <summary>
        /// Returns a string representation of the object
        /// </summary>
        /// <returns>A space-separated list of input values for each feature plus the
        /// value of reward enclosed in braces</returns>
        public override string ToString()
        {
            return ObjectDetection.FromIdToString(id) + ObjectDetection.FromMotorToString(_motor) + ObjectDetection.FromEndToString(REWARDLEVEL);
            //return _color + " " + _size + " " + _hdistr + " " + _vdistr + " (" + _reward + ") " + " " + _end;
        }
    }
}
