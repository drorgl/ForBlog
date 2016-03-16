/*
Updated versions can be found at https://github.com/mikeymckay/google-spreadsheet-javascript
http://mikeymckay.github.com/google-spreadsheet-javascript/sample.html

Changelog:
2012-05-21 - Dror Gluska - Modified to return actual cells from spreadsheet instead of a list.
2016-03-16 - Dror Gluska - Simplify the callback
*/
var GoogleSpreadsheet, GoogleUrl;

GoogleUrl = (function ()
{
    function GoogleUrl(sourceIdentifier)
    {
        this.sourceIdentifier = sourceIdentifier;
        if (this.sourceIdentifier.match(/http(s)*:/))
        {
            this.url = this.sourceIdentifier;
            try
            {
                this.key = this.url.match(/key=(.*?)&/)[1];
            } catch (error)
            {
                this.key = this.url.match(/(cells|list)\/(.*?)\//)[2];
            }
        } else
        {
            this.key = this.sourceIdentifier;
        }
        this.jsonCellsUrl = "http://spreadsheets.google.com/feeds/cells/" + this.key + "/od6/public/basic?alt=json-in-script";
        this.jsonListUrl = "http://spreadsheets.google.com/feeds/list/" + this.key + "/od6/public/basic?alt=json-in-script";
        this.jsonUrl = this.jsonCellsUrl;
    }
    return GoogleUrl;
})();


GoogleSpreadsheet = (function ()
{
    function GoogleSpreadsheet() { }
    GoogleSpreadsheet.prototype.load = function (callback)
    {
        url = this.googleUrl.jsonCellsUrl + "&callback=?";

        $.ajax({
            type: 'GET',
            url: url,
            async: false,
            contentType: "application/json",
            dataType: 'jsonp',
            success: function (json) {
                callback(GoogleSpreadsheet.processData(json))
            },
            error: function (e) {
                console.log("Error retrieving google spreadsheet data",e);
            }
        });

    };
    GoogleSpreadsheet.prototype.url = function (url)
    {
        return this.googleUrl(new GoogleUrl(url));
    };
    GoogleSpreadsheet.prototype.googleUrl = function (googleUrl)
    {
        if (typeof googleUrl === "string")
        {
            throw "Invalid url, expecting object not string";
        }
        this.url = googleUrl.url;
        this.key = googleUrl.key;
        this.jsonUrl = googleUrl.jsonUrl;
        return this.googleUrl = googleUrl;
    };
    return GoogleSpreadsheet;
})();
GoogleSpreadsheet.processData = function (data)
{
    var cell, googleSpreadsheet, googleUrl;
    
    var _i, _len, _ref, _results;
        _ref = data.feed.entry;
        _results = {};
        for (_i = 0, _len = _ref.length; _i < _len; _i++)
        {
            cell = _ref[_i];
            _results[cell.title.$t] = cell.content.$t;
        }
        return _results;
  
};
/* TODO (Handle row based data)
GoogleSpreadsheet.callbackList = (data) ->*/
