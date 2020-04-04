using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Data;
using OSIsoft.AF.PI;

namespace PI_Connect
{
    class UseCase_DataPipe
    {
        public static void Run(PIServer piserver)
        {
            //Constructs a new PIDataPipe to signup for events on a list of PIPoint objects 
            PIDataPipe piDP_A = new PIDataPipe(AFDataPipeType.TimeSeries);
            try
            {
                //List of PIPoints
                var ptListNames = new List<string> { "sinusoid","CDT158", "CDT158Testt" };
                var ptList = PIPoint.FindPIPoints(piserver, ptListNames.AsEnumerable());

                //Find Tags Available in the DataArchive
                foreach (var val in ptListNames)
                {
                    if ((PIPoint.TryFindPIPoint(piserver, val, out PIPoint point) == false))
                        Console.WriteLine("Tag Has been DELETED: {0}", val.ToString());
                }
                Console.ReadLine();

                //PIDataPipe1
                //Moved below line above try block 
                //PIDataPipe piDP_A = new PIDataPipe(AFDataPipeType.TimeSeries);
                //Take Returns a specified number of contiguous elements from the start of a sequence.
                var errPIa = piDP_A.AddSignups(ptList.Take(2).ToList());
                var observerpiA = new Pipe_Observer("observerpiA");
                // registering an Iobserver for ADDataPipeEvent with the PIDataPipe. All the AFDataPipeEvents received by the data pipe will 
                // be sent to the IObserver. 
                piDP_A.Subscribe(observerpiA);

                //using 3 minutes time to gracefully exiting and showing how dispose is working in finally block
                DateTime start = DateTime.Now;

                while (true && DateTime.Now.Subtract(start).Minutes < 3)
                    {
                        bool hasMorePiA;
                    //Get updates.... Trigger retrival of new events    
                    var PIa = piDP_A.GetObserverEvents(100, out hasMorePiA);

                    // out hasMorePiA - Indicates whether there could be more events in the pipe. hasMorePiA is set to true whenever the number of result
                    // events reach maxEventCountPerServer for any one PI Data Archive within the pipe.

                    if (hasMorePiA == true)
                        Console.WriteLine("the number of result events reach maxEventCountPerServer for any one PI Data Archive within the pipe");
                    // while (hasMorePiA) ;

                    }

            }

            catch (Exception ex)
            {
                Logs Err = new Logs();
                Err.MyLogFile(ex);
                Console.WriteLine("An Error has occured for details please check the Log File:'" + ex.Message + "'");
                Console.ReadLine();
            }

            finally
            {
                //close – which will terminate the data pipes connection to the PI Servers associated with the monitored PI Point Object
                //Dispose - which will terminate the data pipes connection to the PI Servers associated with the monitored PI Point Object.
                //This method also releases the resources used by the PIDataPipe

                piDP_A.Close();
                piDP_A.Dispose();
            }

        }
    }


    internal class Pipe_Observer : IObserver<AFDataPipeEvent>
    {

        public IDisposable UnSubscriber;
        private List<AFDataPipeEvent> _pendingEvents;
        private bool _bHasComplete;
        private int _errorCount;
        private string _Name;
        public Pipe_Observer(string name)
        {
            try
            {
                _pendingEvents = new List<AFDataPipeEvent>();
                _bHasComplete = false;
                _errorCount = 0;
                _Name = name;
            }
            catch (Exception ex)
            {
                Logs Err = new Logs();
                Err.MyLogFile(ex);
                Console.WriteLine("An Error has occured for details please check the Log File:'" + ex.Message  + "'");
                Console.ReadLine();
            }

        }
        // IObserver interfaces    
        public void OnNext(AFDataPipeEvent value)
        {
            try
            {
                if (value.Action.ToString() == "Add")
                {
                    AFValue val = value.Value;
                    Console.WriteLine($"{val.Value.ToString()} {val.PIPoint.Name} {val.Timestamp} {value.Action}");
                    _pendingEvents.Add(value);
                }
                else
                {
                    Console.WriteLine("An Event Type Received was :'" + value.Action + "'");
                }

                //https:/techsupport.osisoft.com/Documentation/PI-AF-SDK/html/T_OSIsoft_AF_Data_AFDataPipe.htm
            }
            catch (Exception ex) 
            {
                Logs Err = new Logs();
                Err.MyLogFile(ex);
                Console.WriteLine("An Error has occured for details please check the Log File:'" + ex.Message + "'");
                Console.ReadLine();
            }


        }
        public void OnError(Exception ex)
        {
            _errorCount++;
        }
        public void OnCompleted()
        {
            _bHasComplete = true;
        }
    }
}
