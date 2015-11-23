using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace GraylogArduinoTray
{
    class ElasticSearch
    {
        public WebRequest request;
        public WebResponse response;
        public int numberOfHits = 0; // total number of hits we receive from elastic search
        public string hits;  // some hits that we parse for printing
        public string url, body;
        public bool lastSearchSuccessful;

        public ElasticSearch(string _url, string _body)
        {
            url = _url;
            body = _body;
        }

        public void postSearch()
        {
            try {
                // JSON is always encoded in UTF-8 the internetoverlords tell me
                byte[] jsonBytes = Encoding.UTF8.GetBytes(body);

                request = WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = jsonBytes.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(jsonBytes, 0, jsonBytes.Length);
                dataStream.Close();

                response = request.GetResponse();

                if (response != null)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(String.Format("Server error (HTTP {0}: {1}).", httpResponse.StatusCode, httpResponse.StatusDescription));
                    }

                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SearchResponse));
                    SearchResponse jsonResponse = (SearchResponse)jsonSerializer.ReadObject(response.GetResponseStream());

                    numberOfHits = jsonResponse.hits.total;

                }
                else
                {
                    throw new Exception(string.Format("No response was received wen requesting data from {0} with JSON {1}", url, body));
                }

                lastSearchSuccessful = true;
            }
            catch (Exception e)
            {
                lastSearchSuccessful = false;
                Logger.error(e.ToString());
            }
        }
    }



    // created by http://jsontodatacontract.azurewebsites.net/ from sample JSON response
    //------------------------------------------------------------------------------
    // <auto-generated>
    //     This code was generated by a tool.
    //     Runtime Version:4.0.30319.42000
    //
    //     Changes to this file may cause incorrect behavior and will be lost if
    //     the code is regenerated.
    // </auto-generated>
    //------------------------------------------------------------------------------

        // Type created for JSON at <<root>>
        [System.Runtime.Serialization.DataContractAttribute()]
        public partial class SearchResponse
        {
/*
            [System.Runtime.Serialization.DataMemberAttribute()]
            public int took;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public bool timed_out;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public _shards _shards;
*/
            [System.Runtime.Serialization.DataMemberAttribute()]
            public Hits hits;
        }
/*
        // Type created for JSON at <<root>> --> _shards
        [System.Runtime.Serialization.DataContractAttribute()]
        public partial class _shards
        {

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int total;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int successful;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int failed;
        }
*/
        // Type created for JSON at <<root>> --> hits
        [System.Runtime.Serialization.DataContractAttribute(Name = "hits")]
        public partial class Hits
        {

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int total;

            //[System.Runtime.Serialization.DataMemberAttribute()]
            //public double max_score;

            //[System.Runtime.Serialization.DataMemberAttribute()]
            //public Hits1[] hits;
        }
/*
        // Type created for JSON at <<root>> --> hits --> hits
        [System.Runtime.Serialization.DataContractAttribute(Name = "hits")]
        public partial class Hits1
        {

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string _index;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string _type;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string _id;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public double _score;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public _source _source;
        }
*/
        /*
        // Type created for JSON at <<root>> --> _source
        [System.Runtime.Serialization.DataContractAttribute()]
        public partial class _source
        {

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int RecordNumber;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string Channel;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string gl2_source_node;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string EventReceivedTime;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string version;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string timestamp;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string SourceModuleName;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string Severity;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int level;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string _id;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int SeverityValue;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string gl2_source_input;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public long Keywords;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string SourceModuleType;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string full_message;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int ThreadID;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int EventID;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string EventType;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int ProcessID;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string message;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string Category;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string SourceName;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string source;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public int Task;

            [System.Runtime.Serialization.DataMemberAttribute()]
            public string[] streams;
        }
*/
}
