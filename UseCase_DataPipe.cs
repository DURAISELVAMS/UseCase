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
            //PIDataPipe1
            PIDataPipe piDP_A = new PIDataPipe(AFDataPipeType.TimeSeries);
            try
            {
                //List of PIPoints
                var ptListNames = new List<string> { "sinusoid", "CDT158Test" };
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
                var errPIa = piDP_A.AddSignups(ptList.Take(2).ToList());
                var observerpiA = new PipeObserver("observerpiA");
                piDP_A.Subscribe(observerpiA);

                //using 3 minutes time to gracefully exiting and showing how dispose is working in finally block
                DateTime start = DateTime.Now;
                while (true && DateTime.Now.Subtract(start).Minutes < 3)
                    {
                        bool hasMorePiA;
                        //Get updates    
                        var PIa = piDP_A.GetObserverEvents(100, out hasMorePiA);
                    }

            }
            catch (Exception)
            {

                throw;
            }

            finally
            {
                //close – which will terminate the data pipes connection to the PI Servers associated with the monitored PI Point Object
                //Dispose - which will terminate the data pipes connection to the PI Servers associated with the monitored PI Point Object.This method also releases the resources used by the PIDataPipe

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
            catch (Exception)
            {
                throw;
            }

        }
        // IObserver interfaces    
        public void OnNext(AFDataPipeEvent value)
        {
            try
            {
                AFValue val = value.Value;
                Console.WriteLine($"{val.Value.ToString()} {val.PIPoint.Name} {val.Timestamp}");
                _pendingEvents.Add(value);
            }
            catch (Exception)
            {
                throw;
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
