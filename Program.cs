﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

// from http://random.org
namespace RandomOrg {
  class Program {
    static void Main(string[] args) {
      MainAsync(args).GetAwaiter().GetResult();
    }

    static async Task MainAsync(string[] args) {
      if( args.Length != 1) {
        Console.WriteLine("Usage: RandomOrg api-key\r\nYou can get an API Key from https://api.random.org/api-keys");
        return;
      }

      var data = await GetTrulyRandomNumbers(args[0], 10, 1, 10);
      Console.WriteLine($"truly random numbers: {string.Join(',', data)}");
    }

    static async Task<IEnumerable<int>> GetTrulyRandomNumbers(string apiKey, int n, int min, int max) {
      // from https://api.random.org/json-rpc/1/introduction
      var url = "https://api.random.org/json-rpc/1/invoke";
      using (var client = new HttpClient()) {
        var req = JsonConvert.SerializeObject(
          new {
            jsonrpc = "2.0",
            method = "generateIntegers",
            @params = new {
              apiKey = apiKey,
              n = n,
              min = min,
              max = max,
            },
            id = "1"
          }
        );

        // result example:
        /*
          "jsonrpc": "2.0",
          "result": {
            "random": {
              "data": [ 8, 1 ],
              "completionTime": "2017-11-22 23:48:25Z"
            },
            "bitsUsed": 33,
            "bitsLeft": 249886,
            "requestsLeft": 992,
            "advisoryDelay": 1290
          },
          "id": "1"
        }
        */
        var res = await client.PostAsync(url, new StringContent(req, Encoding.UTF8, "application/json-rpc"));
        var json = await res.Content.ReadAsStringAsync();
        Debug.WriteLine($"random.org result: {json}");
        var obj = JObject.Parse(json);
        return ((JArray)obj["result"]["random"]["data"]).Select(d => (int)d);
      }
    }
  }
}

