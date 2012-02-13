using System;

namespace AsteroidsGame
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MainClass game = new MainClass())
            {
                game.Run();
            }
            //try
            //{
            //    using (MainClass game = new MainClass())
            //    {
            //        game.Run();
            //    }
            //}
            //catch (Microsoft.Xna.Framework.NoSuitableGraphicsDeviceException)
            //{
            //    System.Windows.Forms.MessageBox.Show("Your VGA card is not supported\nContact me on hassanselim0@hotmail.com"
            //        + "\nSorry for your disatisfaction", "Error !",
            //        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            //}
            //catch (Exception e)
            //{
            //    System.Windows.Forms.MessageBox.Show("Unexpected Error\nContact me on hassanselim0@hotmail.com"
            //        + "\nSorry for your disatisfaction", "Error !",
            //        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            //    System.Windows.Forms.MessageBox.Show("Error Message:\n" + e.Message + "\nStack Trace:\n" + e.StackTrace
            //        + "\n\nTake a ScreenShot for this error and send it to me on hassanselim0@hotmail.com",
            //        "Error Details, Send them to me",
            //        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            //}
        }
    }
}

