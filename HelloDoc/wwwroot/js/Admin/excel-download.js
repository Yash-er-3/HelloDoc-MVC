
//for export all GamepadButton

$('.exportall-download').click(function () {
    console.log("Allexport")
    $.ajax({
        url: 'Admin/ExportAllDownload',
        type: 'GET',
        success: function (data) {
            console.log(data);
            // Include the xlsx library
        /*    var XLSX = require('xlsx');*/

            // Convert the JSON data to a worksheet
            var worksheet = XLSX.utils.json_to_sheet(data);

            // Create a new workbook, with the newly created worksheet
            var workbook = XLSX.utils.book_new();
            XLSX.utils.book_append_sheet(workbook, worksheet, 'Sheet1');

            // Write the workbook to a file
            XLSX.writeFile(workbook, 'output.xlsx');
        }
    })
})

//for export button
$('button.download').on('click', function () {

    console.log("excel-download")
    // Get the table associated with this tab
    //var tableId = $(this).data('table');
    var tabledata = $('#dtBasicExample');
    console.log("table" + tabledata)
    // Convert the table data to JSON
    var data = tableToJson(tabledata);

    // Use xlsx.js to convert the JSON to an Excel file
    var wb = XLSX.utils.book_new();
    var ws = XLSX.utils.json_to_sheet(data);
    XLSX.utils.book_append_sheet(wb, ws, "Sheet1");

    // Use FileSaver.js to save the file
    var wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'binary' });
    saveAs(new Blob([s2ab(wbout)], { type: "application/octet-stream" }), 'data.xlsx');
});
function tableToJson(tabledata) {

    var data = [];

    // Get the headers
    var headers = [];
    tabledata.find('th').each(function () {
        headers.push($(this).text().trim());
    });
    console.log("headers : " + headers);
  

    // Get the row data
    tabledata.find('tr').each(function () {
        var row = {}
        $(this).find('td').each(function (i) {
            row[headers[i]] = $(this).text().trim();
        });

        console.log(row)
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
