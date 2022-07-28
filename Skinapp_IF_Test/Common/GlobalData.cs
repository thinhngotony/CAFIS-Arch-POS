using System.Collections.Generic;

namespace Vjp.Skinapp_IF_Test.Common
{
    internal class GlobalData
    {
        public static string AppName = "SKINAPP_INTERFACE_TEST";
        public static string AppPath;
        public static Utilities.State CurrentState;
        public static string SecondState;

        public static Utilities.PosIFConfig PosIFConfig = new Utilities.PosIFConfig
        {
            msg_name = "Skinapp_Interface_Test",
            pos_process_name = "POS_INTERFACE_TEST",
            pos_window_name = "Mainview",
            pos_btn_loc = new int[] { 150, 50 },
            pos_timeout = 5000,
            from_pos_dat = "C:\\WEBPOS\\Saturn\\POS\\FromPos.dat",
            to_pos_dat = "C:\\WEBPOS\\Saturn\\POS\\ToPos.dat"
        };
        public static Utilities.FromPos Data_FromPosDat = new Utilities.FromPos();
        public static Utilities.ToPos Data_ToPosDat = new Utilities.ToPos();

        public static string[] ServiceNameCorrect = new string[] { "Edy", "iD", "QUICPay", "Suica", "WAON", "nanaco" };

        public static string PosPath = @"\Pos";
        public static string LogPath = @"\Log";
        public static string ConfigPath = @"\Config";

        public static string FromPosPath = PosPath + @"\FromPos.dat";
        public static string ToPosPath = PosPath + @"\ToPos.dat";

        public static string PosIFConfigPath = ConfigPath + @"\PosIFConfig.json";
    }
}
