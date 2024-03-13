// Assuming you have jQuery
$('button.download').on('click', function () {

    console.log("excel-download")
    // Get the table associated with this tab
    var tableId = $(this).data('table');
    console.log(tableId);
    var table = $('#dtBasicExample');
    console.log("table" + table)
    // Convert the table data to JSON
    var data = tableToJson(table);

    // Use xlsx.js to convert the JSON to an Excel file
    var wb = XLSX.utils.book_new();
    var ws = XLSX.utils.json_to_sheet(data);
    XLSX.utils.book_append_sheet(wb, ws, "Sheet1");

    // Use FileSaver.js to save the file
    var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
    saveAs(new Blob([s2ab(wbout)], { type: "application/octet-stream" }), 'data.xlsx');
});

function tableToJson(table) {

    //var data = [];

    //// first row needs to be headers
    //var headers = [];
    //for (var i = 0; i < table.rows[0].cells.length; i++) {
    //    headers[i] = table.rows[0].cells[i].innerHTML.toLowerCase().replace(/ /gi, '');
    //}

    //// go through cells
    //for (var i = 1; i < table.rows.length; i++) {

    //    var tableRow = table.rows[i];
    //    var rowData = {};

    //    for (var j = 0; j < tableRow.cells.length; j++) {

    //        rowData[headers[j]] = tableRow.cells[j].innerHTML;

    //    }

    //    data.push(rowData);
    //}

    //return data;
    var data = [];

    // Get the headers
    var headers = [];
    table.find('th').each(function () {
        headers.push($(this).text().trim());
    });
    console.log("headers : " + headers);
    headers.shift();
    headers.pop();

    // Get the row data
    table.find('tr').each(function () {
        var row = {};
        $(this).find('td').each(function (i) {
            row[headers[i]] = $(this).text().trim();
        });
        
        data.push(row);
    });

    return data;
}

function s2ab(s) {
    var buf = new ArrayBuffer(s.length);
    var view = new Uint8Array(buf);
    for (var i = 0; i < s.length; i++) view[i] = s.charCodeAt(i) & 0xFF;
    return buf;
}
