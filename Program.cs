using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF;
using OSIsoft.AF.UnitsOfMeasure;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Data;
using OSIsoft.AF.Search;
using OSIsoft.AF.Time;
using OSIsoft.AF.PI;
using System.Configuration;
using System.Collections.Specialized;

namespace PI_Connect
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                PISystems piSystems = new PISystems();
                string AFServer = ConfigurationManager.AppSettings.Get("AFServer");
                PISystem assetServer = piSystems[AFServer];


                PIServers piServers = new PIServers();
                string PIServer = ConfigurationManager.AppSettings.Get("DataArchiveServer");
                PIServer piServer = piServers[PIServer];

                //AFElement.LoadAttributes()
                if (assetServer == null)
                {
                    Console.WriteLine("Cannot Find AF Server: {0}", AFServer);
                    Console.ReadLine();
                    Environment.Exit(0);
                }
                if (piServer == null)
                {
                    Console.WriteLine("Cannot Find PI Data Server: {0}", PIServer);
                    Console.ReadLine();
                    Environment.Exit(0);
                }

                if (assetServer != null && piServer != null)
                {
                    //AFDatabase database = assetServer.Databases["Demo UOG Well Drilling & Completion Monitoring"];
                    //AFDatabase database = assetServer.Databases["AARA OEE Demo"];


                    // PrintElementTemplates(database);
                    // PrintAttributeTemplates(database, "Batch Context Template");
                    // PrintEnergyUOMs(assetServer);
                    // PrintEnumerationSets(database);
                    // PrintCategories(database);
                    // FindMetersBYTemplate(database, "Line");

                    // TimeSpan test = new TimeSpan(0, 1, 0);
                    //Program6.PrintInterpolated(database, "Inlet Pump", "14-Nov-18 16:15:00", "14-Nov-18 17:15:00.000000", test);
                    //Program6.CreateFeedersRootElement(database);
                    // Program6.CreateFeederElements(database);

                    //DataPipeSubscribeExample.Run(piServer);
                    Console.WriteLine("Connected Successfully to PI Server: {0} and AF Server: {1}", PIServer, AFServer);
                    //Subscribe.DataPipeSubscribeExample.Run1();
                   UseCase_DataPipe.Run(piServer);
                    Console.ReadLine();
                }

            }
            catch (Exception ex)
            {
                Logs Err = new Logs();
                Err.MyLogFile(ex);
                Console.WriteLine("An Error has occured for details please check the Log File: '" + ex.Message + "'");
                Console.ReadLine();
            }


        }

        static void PrintElementTemplates(AFDatabase database)
        {
            try
            {
                Console.WriteLine("Print Element Templates");
                AFNamedCollectionList<AFElementTemplate> elemTemplates = database.ElementTemplates.FilterBy(typeof(AFElement));
                foreach (AFElementTemplate elemTemp in elemTemplates)
                {
                    Console.WriteLine("Name: {0}; Categories: {1}", elemTemp.Name, elemTemp.CategoriesString);

                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Logs Err = new Logs();
                Err.MyLogFile(ex);
                Console.WriteLine("An Error has occured for details please check the Log File: '" + ex.Message + "'");
                Console.ReadLine();
            }


        }

        static void PrintAttributeTemplates(AFDatabase database, string elemTempName)
        {
            try
            {
                Console.WriteLine("Print Attribute Templates for Element Template:     {0}", elemTempName);
                AFElementTemplate elemTemp = database.ElementTemplates[elemTempName];
                foreach (AFAttributeTemplate attrTemp in elemTemp.AttributeTemplates)
                {
                    string drName = attrTemp.DataReferencePlugIn == null ? "None" : attrTemp.DataReferencePlugIn.Name;
                    Console.WriteLine("Name: {0}, DRPlugin: {1}", attrTemp.Name, drName);
                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Logs Err = new Logs();
                Err.MyLogFile(ex);
                Console.WriteLine("An Error has occured for details please check the Log File: '" + ex.Message + "'");
                Console.ReadLine();
            }

        }

        static void PrintEnergyUOMs(PISystem system)
        {
            try
            {
                Console.WriteLine("Print Energy UOMS");
                UOMClass uomClass = system.UOMDatabase.UOMClasses["Specific Entropy, Specific Heat Capacity"];


                foreach (UOM uom in uomClass.UOMs)
                {
                    Console.WriteLine("UOM: {0}, Abbrevation: {1}", uom.Name, uom.Abbreviation);
                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Logs Err = new Logs();
                Err.MyLogFile(ex);
                Console.WriteLine("An Error has occured for details please check the Log File: '" + ex.Message + "'");
                Console.ReadLine();
            }

        }

        static void PrintEnumerationSets(AFDatabase database)
        {
            try
            {
                Console.WriteLine("Print Enumeration Sets");

                AFEnumerationSets enumSets = database.EnumerationSets;
                foreach (AFEnumerationSet enumSet in enumSets)
                {
                    Console.WriteLine(enumSet.Name);
                    foreach (AFEnumerationValue state in enumSet)
                    {
                        int stateValue = state.Value;
                        string stateName = state.Name;
                        Console.WriteLine("{0} - {1}", stateValue, stateName);
                    }


                }
            }
            catch (Exception ex)
            {
                Logs Err = new Logs();
                Err.MyLogFile(ex);
                Console.WriteLine("An Error has occured for details please check the Log File: '" + ex.Message + "'");
                Console.ReadLine();
            }

        }

        static void PrintCategories(AFDatabase database)
        {
            try
            {
                Console.WriteLine("Print Categories");
                AFCategories elemCategories = database.ElementCategories;
                AFCategories attrCategories = database.AttributeCategories;


                Console.WriteLine("Element Categories");
                foreach (AFCategory category in elemCategories)
                {
                    Console.WriteLine("{0}", category.Name);

                }

                Console.WriteLine();
                Console.WriteLine("Attribute Categories");
                foreach (AFCategory category in attrCategories)
                {
                    Console.WriteLine(category.Name);

                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Logs Err = new Logs();
                Err.MyLogFile(ex);
                Console.WriteLine("An Error has occured for details please check the Log File: '" + ex.Message + "'");
                Console.ReadLine();
            }


        }

        // Exercise 1: Find meters by template 
        static void FindMetersBYTemplate(AFDatabase database, string templateName)
        {
            try
            {
                Console.WriteLine("Find Meters By Template: {0}", templateName);
                using (AFElementSearch elementQuery = new AFElementSearch(database, "TemplateSearch", string.Format("template:\"{0}\"", templateName)))
                {
                    elementQuery.CacheTimeout = TimeSpan.FromMinutes(5);
                    int countDerived = 0;
                    foreach (AFElement element in elementQuery.FindElements())
                    {
                        Console.WriteLine("Element: {0}, Template: {1}", element.Name, element.Template);
                        if (element.Template.Name != templateName)
                            countDerived++;
                    }
                    Console.WriteLine("Found {0} derived Templates", countDerived);
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Logs Err = new Logs();
                Err.MyLogFile(ex);
                Console.WriteLine("An Error has occured for details please check the Log File: '" + ex.Message + "'");
                Console.ReadLine();
            }

        }

        // Exercise 3: Find meters with above-average usage: 

        static void FindMetersAboveUsage(AFDatabase database, double val)
        {
            try
            {
                Console.WriteLine("Find Meters Above Usuage: {0}", val);
                string templateName = "MeterBasic";
                string attributeName = "Energy Usage";
                AFElementSearch elementQuery = new AFElementSearch(database, " AttributeValueGTSearch", string.Format("template:\"{0}\" \"|{1}\":>{2}", templateName, attributeName, val));
                int countNames = 0;
                foreach (AFElement element in elementQuery.FindElements())
                {
                    Console.WriteLine("{0}{1}", countNames++ == 0 ? string.Empty : ", ", element.Name);
                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Logs Err = new Logs();
                Err.MyLogFile(ex);
                Console.WriteLine("An Error has occured for details please check the Log File: '" + ex.Message + "'");
                Console.ReadLine();
            }

        }


    }
}
