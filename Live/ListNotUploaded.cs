using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Live
{
    class ListNotUploaded
    {

        public async void AutoUpload()
        {
            var path = new Other().NotDomePath;

            while (true)
            {
                Thread.Sleep(1000);

                if (new FileInfo(path).Length > 0)
                {



                    string line1 = File.ReadLines(path).First();

                    if (!string.IsNullOrWhiteSpace(line1)) // PAZIUREK DEL ! OPERATOR NEZINAU AR TIKRAI VEIKIA!!!!!!!!
                    {
                        try
                        {
                            Upload upl = new Upload();
                            Console.WriteLine("Sending To Upload Function");
                            await upl.Run(line1); // REIKIA SUTAISYKTI GET TIKRAI VEIKTU REPEAT UPLOAD ON THREAD!!!
                        }

                        catch (Exception ex3)
                        {
                            Console.WriteLine("Auto upload failed error: " + ex3.Message);
                            Console.WriteLine("thread sleep ??? is this what happens when your quota is exceeded?");
                        }


                    }
                }
        } // WHILE BRACKET

        }





    }
}
