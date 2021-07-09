// This file is part of the DSharpPlus project.
//
// Copyright (c) 2015 Mike Santiago
// Copyright (c) 2016-2021 DSharpPlus Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.Net;

namespace DSharpPlus
{
    internal class RestManager : IManager
    {
        public DiscordConfiguration Configuration { get; }
        public IRatelimiter Ratelimiter { get; }

        public RestManager(DiscordConfiguration configuration)
        {
            this.Configuration = configuration;
            this.Ratelimiter = new RestRatelimiter(configuration);
        }

        public Task<DiscordGuild> GetGuildAsync(ulong guildId, bool? with_counts)
        {
            var route = this.PrepareRoute($"{Endpoints.GUILDS}/{guildId}", with_counts);
            var request = new RestRequest("GET", route);
            return this.Ratelimiter.QueueAsync<DiscordGuild>(request);            
        }

        private Uri PrepareRoute(string route, params object[] @params)
        {
            var url = Endpoints.BASE_URI + route;

            if (@params != null)
            {
                var urlParams = this.BuildQueryString(@params);
                url += urlParams;
            }

            return new Uri(url);
        }

        private string BuildQueryString(object[] @params, bool post = false)
        {
            if (@params == null || @params.Length == 0)
                return string.Empty;

            var vals_collection = @params.Select(obj =>
            {
                var xkvp = obj.ToString();
                return $"{WebUtility.UrlEncode(xkvp)}={WebUtility.UrlEncode(xkvp)}";
            });

            var vals = string.Join("&", vals_collection);
            return !post ? $"?{vals}" : vals;
        }
    }
}
