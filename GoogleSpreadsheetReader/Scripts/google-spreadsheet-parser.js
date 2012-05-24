/*
GoogleSpreadsheetParse - parses results from google-spreadsheet.js into a workable object.

Changelog:
2012-05-21 - Dror Gluska - Basic parsing
2012-05-22 - Dror Glusk - Basic filtering

Usage:

//initialize a new instance and parse the result.
var sparser = new GoogleSpreadsheetParser(result);

//get all rows
var allrows = sparser.GetAllRows();

//Get just one row - its 1 based (same as the spreadsheet).
//GetRow 1 will usually retrieve the title.
var row = sparser.GetRow(1);

//return number of columns in the spreadsheet
var columns = sparser.ColumnCount();

//return number of rows in the spreadsheet
var rows = sparser.RowCount();

//in a spreadsheet that the first row is a title:
//-------------------------------------------------

//get the column code (A,B,C..) from the name 
var columnCode = sparser.GetColumnCode("column name");

//Filter rows by column name
var filteredRows = sparser.FilterByColumn(sparser.GetAllRows(),"column name","column value");

//Get First row, if none available, return null
var firstRow = sparser.First(sparser.GetAllRows());

//Get Distinct value list by column name, it will include the column name in the list, you may remove it with delete later.
var distinctColumnValues = sparser.DistinctByColumn(sparser.GetAllRows(),"column name");

//Gets a cell value, if multiple rows were passed, it will return the first
var cellvalue = sparser.GetValue(sparser.GetAllRows(),"column name");

//Orders the rows by column name (not implemented)
var orderedrows = sparser.SortByColumn(sparser.GetAllRows(),"column name",true);

*/


GoogleSpreadsheetParser = (function ()
{
    var jsondoc = {};

    Rows = {};
    Columns = {};

    columncount = 0;
    rowcount = 0;

    function GoogleSpreadsheetParser(jsonDocument) { jsondoc = jsonDocument; parsedoc(); }

    function parsedoc()
    {
        for (var key in jsondoc.data)
        {
            var column = key.replace(/[^a-z]/gi, "");
            var row = key.replace(/[^0-9]/gi, "");
            if (Columns[column] == null)
            {
                Columns[column] = true;
                this.columncount++;
            }

            if (Rows[row] == null)
            {
                Rows[row] = {};
                this.rowcount++;
            }

            Rows[row][column] = jsondoc.data[key];
        }
    }

    GoogleSpreadsheetParser.prototype.GetAllRows = function ()
    {
        return Rows;
    };

    GoogleSpreadsheetParser.prototype.GetRow = function (rowNumber)
    {
        return Rows[rowNumber];
    };

    GoogleSpreadsheetParser.prototype.ColumnCount = function ()
    {
        return columncount;
    };

    GoogleSpreadsheetParser.prototype.RowCount = function ()
    {
        return rowcount;
    };

    GoogleSpreadsheetParser.prototype.GetColumnCode = function (columnName)
    {
        for (var ccode in Rows[1])
        {
            if (Rows[1][ccode] == columnName)
            {
                return ccode;
            }
        }
    }

    GoogleSpreadsheetParser.prototype.FilterByColumn = function (fromrows, columnName, columnValue)
    {
        var result = {};

        var columnCode = this.GetColumnCode(columnName);

        for (var row in fromrows)
        {
            if (fromrows[row][columnCode] == columnValue)
            {
                result[row] = fromrows[row];
            }
        }

        return result;
    }

    GoogleSpreadsheetParser.prototype.First = function (fromrows)
    {
        for (var row in fromrows)
        {
            return fromrows[row];
        }
        return null;
    }

    GoogleSpreadsheetParser.prototype.DistinctByColumn = function (fromrows, columnName)
    {
        var result = {};

        var columnCode = this.GetColumnCode(columnName);

        for (var row in fromrows)
        {
            if (result[fromrows[row][columnCode]] == null)
                result[fromrows[row][columnCode]] = 0;

            result[fromrows[row][columnCode]]++;
        }

        return result;
    }

    GoogleSpreadsheetParser.prototype.GetValue = function (fromrow, columnName)
    {
        var columnCode = this.GetColumnCode(columnName);
        return this.First(fromrow)[columnCode];
    }

    GoogleSpreadsheetParser.prototype.SortByColumn = function (fromrows, columnName, ascending)
    {
        throw "not implemented";
        var columnCode = this.GetColumnCode(columnName);

        var returnarray = [];

        for (var row in fromrows) returnarray.push(fromrows[row]);
        returnarray.sort(function () { return arguments[0][columnCode] - arguments[1][columnCode]; });

        return returnarray;
    }

    return GoogleSpreadsheetParser;
})();