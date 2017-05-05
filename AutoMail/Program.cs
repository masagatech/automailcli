using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using System.Collections;
using System.Configuration;
using System.IO;

namespace AutoMail
{
    class Program
    {
        static Dictionary<string, string> CommandsDetails = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            AddCommands();

            if (args.Length > 0)
            {
                DoAction(args[0]);
                Console.WriteLine("Exiting......");
                Thread.Sleep(1000);

            }
            else
            {
                Console.WriteLine("Invalid Call");
                foreach (var item in CommandsDetails)
                {
                    Console.WriteLine(item.Key.PadRight(20) + " - " + item.Value);
                }
                Thread.Sleep(1000);

            }



        }


        private static void AddCommands()
        {
            CommandsDetails.Add("weeklycollection", "Send Weekly collection data. to respective superzone.");
            CommandsDetails.Add("target", "Send Monthly Target data. to respective superzone.");
            CommandsDetails.Add("cms", "Send Weekly collection data. to respective email.");
            CommandsDetails.Add("missing-receipts", "Send missing receipts data. to respective superzone.");
            CommandsDetails.Add("customeros", "Send Outstanding to the customer.");
            CommandsDetails.Add("customerinv", "Send PDF Invoice to the customer.");

        }

        static void DoAction(string ActionName)
        {string mailserver = "";

            switch (ActionName)
            {
                case "-h":
                    foreach (var item in CommandsDetails)
                    {
                        Console.WriteLine(item.Key.PadRight(20) + " - " + item.Value);
                    }

                    break;
                case "weeklycollection":
                    if (ConfigurationManager.AppSettings["weeklycollection"] != null)
                    {
                        string CollectionUrl = ConfigurationManager.AppSettings["weeklycollection"].ToString();
                        string[] otherdetails = CollectionUrl.Split(';');


                        if (otherdetails.Length > 1)
                        {
                            mailserver = otherdetails[1].ToString();
                        }

                        CallService(ActionName, otherdetails[0].ToString(), "", mailserver);
                    }
                    else
                    {
                        Log(DateTime.Now.ToString() + "\\log.txt", "Weekly collection Url not specified!");
                        Console.WriteLine("Weekly collection Url not specified!");
                    }
                    break;
                case "target":
                    if (ConfigurationManager.AppSettings["target"] != null)
                    {
                        string TargetUrl = ConfigurationManager.AppSettings["target"].ToString();
                        string[] otherdetails = TargetUrl.Split(';');


                        if (otherdetails.Length > 1)
                        {
                            mailserver = otherdetails[1].ToString();
                        }

                        CallService(ActionName, otherdetails[0].ToString(), "", mailserver);
                    }
                    else
                    {
                        Log(DateTime.Now.ToString() + "\\log.txt", "Target Url not specified!");
                        Console.WriteLine("Target Url not specified!");
                    }
                    break;
                case "cms":
                    if (ConfigurationManager.AppSettings["cms"] != null)
                    {
                        string CMS = ConfigurationManager.AppSettings["cms"].ToString();
                        string[] otherdetails = CMS.Split(';');


                        if (otherdetails.Length > 1)
                        {
                            mailserver = otherdetails[1].ToString();
                        }

                        CallService(ActionName, otherdetails[0].ToString(), "", mailserver);
                    }
                    else
                    {
                        Log(DateTime.Now.ToString() + "\\log.txt", "CMS Url not specified!");
                        Console.WriteLine("CMS Url not specified!");
                    }
                    break;
                case "missing-receipts":
                    if (ConfigurationManager.AppSettings["missing-receipts"] != null)
                    {
                        string missingreceipts = ConfigurationManager.AppSettings["missing-receipts"].ToString();

                        string[] otherdetails = missingreceipts.Split(';');


                        if (otherdetails.Length > 1)
                        {
                            mailserver = otherdetails[1].ToString();
                        }

                        CallService(ActionName, otherdetails[0].ToString(), "", mailserver);
                    }
                    else
                    {
                        Log(DateTime.Now.ToString() + "\\log.txt", "MissingReceipt Url not specified!");
                        Console.WriteLine("MissingReceipt Url not specified!");
                    }
                    break;
                case "customeros":
                    if (ConfigurationManager.AppSettings["customeros"] != null)
                    {
                        string customeros = ConfigurationManager.AppSettings["customeros"].ToString();

                        string[] otherdetails = customeros.Split(';');
                        
                        
                        if(otherdetails.Length>1){
                        mailserver=otherdetails[1].ToString();
                        }

                        CallService(ActionName, otherdetails[0].ToString(), "", mailserver);
                    }
                    else
                    {
                        Log(DateTime.Now.ToString() + "\\log.txt", "Customeros Url not specified!");
                        Console.WriteLine("customeros Url not specified!");
                    }
                    break;
                case "customerinv":
                    if (ConfigurationManager.AppSettings["customerinv"] != null)
                    {
                        string customerinv = ConfigurationManager.AppSettings["customerinv"].ToString();

                        string[] otherdetails = customerinv.Split(';');


                        if (otherdetails.Length > 1)
                        {
                            mailserver = otherdetails[1].ToString();
                        }

                        CallService(ActionName, otherdetails[0].ToString(), "", mailserver);
                    }
                    else
                    {
                        Log(DateTime.Now.ToString() + "\\log.txt", "Customeros Url not specified!");
                        Console.WriteLine("customeros Url not specified!");
                    }
                    break;
                default:
                    Console.WriteLine("Invalid Parameter");
                    Thread.Sleep(3000);
                    break;
            }
        }

        static void CallService(string actionName, string url, string param, string mailserver)
        {
            try
            {
                Console.WriteLine("Calling Service");
                Console.WriteLine(actionName);
                WebClient1 obj = new WebClient1();
                obj.Url = url;
                switch (actionName)
                {
                    case "cms":
                        //nothing
                        obj.CallService("");
                        break;
                    default:
                        SendMailJob(actionName, obj.CallService(param), mailserver);
                        break;
                }
                obj.Dispose();
            }
            catch (Exception ex)
            {
                Log(DateTime.Now.ToString() + "\\log.txt", "Error 223: " + ex.Message.ToString());
                Console.WriteLine(ex.Message.ToString());
            }
        }

        static void SendMailJob(string Action, string JobData, string mailserver)
        {

            switch (Action)
            {

                default:
                    if (JobData.StartsWith("Error"))
                    {

                        Console.WriteLine(JobData);
                        Log(DateTime.Now.ToString() + "\\log.txt", JobData);
                        break;
                    }

                    Newtonsoft.Json.Linq.JArray r = (Newtonsoft.Json.Linq.JArray)JsonConvert.DeserializeObject(JobData);
                    bool isFirst = true;
                    var FirstItem = r[0];
                    foreach (var item in r)
                    {
                        string filename = "";
                        string log = "";
                        try
                        {
                            if (!isFirst)
                            {
                                if (Action == "customerinv")
                                {
                                    if (item["Desc"] != null)
                                    {
                                        FirstItem["Body"]=item["Desc"];
                                    }
                                }
                                    string attchmentPath = item["Attachment"].ToString();
                                    string[] attch = attchmentPath.Split(',');
                                    filename = Path.GetDirectoryName(attch[0]) + "\\log.txt";
                                    Console.WriteLine("Sending Mail");
                                    log = string.Concat("Sub: ", item["Subject"].ToString(), ",ToEmail: ", item["EmailId"].ToString(), ",Att: ", item["Attachment"].ToString(), ",CC: ", FirstItem["CC"].ToString(), ",BCC: ", FirstItem["BCC"].ToString());
                                    mailsend.sendmail(mailserver, item["Subject"].ToString(), item["EmailId"].ToString(), FirstItem["Body"].ToString().Replace("<sname>", item["Name"].ToString()).Replace("<datetime>", DateTime.Now.ToString("dd-MMM-yyyy")).Replace(System.Environment.NewLine, "</Br>"), item["Attachment"].ToString(), FirstItem["CC"].ToString(), FirstItem["BCC"].ToString());
                                    Console.WriteLine("Mail Sent");
                                    log += "," + item["EmailId"].ToString() == "" ? "not sent" : "sent" + ",Success";
                                
                            }
                            isFirst = false;

                        }
                        catch (Exception ex)
                        {
                            log += ",Error: " + ex.Message;
                        }
                        Log(filename, log);

                    }
                    break;
            }


        }

        private static void Log(string FileName, string data)
        {
            try
            {
                Console.WriteLine(data);
                File.AppendAllText(FileName, DateTime.Now.ToString() + " ==> " + data + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }

        }


    }
}
