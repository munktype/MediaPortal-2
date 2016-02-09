﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace MediaPortal.Extensions.OnlineLibraries.Libraries.Trakt.Web
{

  public class ExtendedWebClient : WebClient
  {
    public int Timeout { private get; set; }

    protected override WebRequest GetWebRequest(Uri address)
    {
      WebRequest request = base.GetWebRequest(address);
      if (request != null)
        request.Timeout = Timeout;
      return request;
    }

    public ExtendedWebClient()
    {
      Timeout = 100000; // the standard HTTP Request Timeout default
    }
  }

  public static class TraktWeb
  {
    public static readonly Dictionary<string, string> _customRequestHeaders = new Dictionary<string, string>();

    #region Events
    public delegate void OnDataSendDelegate(string url, string postData);
    public delegate void OnDataReceivedDelegate(string response);
    public delegate void OnDataErrorReceivedDelegate(string error);

    public static event OnDataSendDelegate OnDataSend;
    public static event OnDataReceivedDelegate OnDataReceived;
    public static event OnDataErrorReceivedDelegate OnDataErrorReceived;
    #endregion

    public static string GetFromTrakt(string address, bool requairedUsername = false, string method = "GET")
    {
      if (OnDataSend != null)
        OnDataSend(address, null);

      var request = WebRequest.Create(address) as HttpWebRequest;

      request.KeepAlive = true;
      request.Method = method;
      request.ContentLength = 0;
      request.Timeout = 120000;
      request.ContentType = "application/json";
      request.UserAgent = UserAgent;
      foreach (var header in _customRequestHeaders)
      {
        request.Headers.Add(header.Key, header.Value);
      }

      if (requairedUsername)
      {
        request.Headers.Add("trakt-user-login", "me");
      }
      try
      {
        WebResponse response = (HttpWebResponse)request.GetResponse();
        if (response == null) return null;

        Stream stream = response.GetResponseStream();
        StreamReader reader = new StreamReader(stream);
        string strResponse = reader.ReadToEnd();

        if (OnDataReceived != null)
          OnDataReceived(strResponse);

        stream.Close();
        reader.Close();
        response.Close();

        return strResponse;
      }
      catch (WebException e)
      {
        if (OnDataErrorReceived != null)
          OnDataErrorReceived(e.Message);

        return null;
      }
    }

    public static string PostToTrakt(string address, string postData, bool logRequest = true)
    {
      if (OnDataSend != null && logRequest)
        OnDataSend(address, postData);

      byte[] data = new UTF8Encoding().GetBytes(postData);

      var request = WebRequest.Create(address) as HttpWebRequest;
      request.KeepAlive = true;

      request.Method = "POST";
      request.ContentLength = data.Length;
      request.Timeout = 120000;
      request.ContentType = "application/json";
      request.UserAgent = UserAgent;
      foreach (var header in _customRequestHeaders)
      {
        request.Headers.Add(header.Key, header.Value);
      }

      try
      {
        // post to trakt
        Stream postStream = request.GetRequestStream();
        postStream.Write(data, 0, data.Length);

        // get the response
        var response = (HttpWebResponse)request.GetResponse();
        if (response == null) return null;

        Stream responseStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(responseStream);
        string strResponse = reader.ReadToEnd();

        if (OnDataReceived != null)
          OnDataReceived(strResponse);

        // cleanup
        postStream.Close();
        responseStream.Close();
        reader.Close();
        response.Close();

        return strResponse;
      }
      catch (WebException e)
      {
        if (OnDataErrorReceived != null)
          OnDataErrorReceived(e.Message);

        return null;
      }
    }

    public static string UserAgent
    {
      get
      {
        return string.Format("TraktForMP2/{0}", Version);
      }
    }

    private static string Version
    {
      get
      {
        return Assembly.GetCallingAssembly().GetName().Version.ToString();
      }
    }
  }
}
