function printElement(element, title)
{
    var data = $(element).html();
    var windowPrint = window.open('', title, '');
    windowPrint.document.write('<html><head><title>' + title + '</title>');
    windowPrint.document.write('<link rel="stylesheet" href="../../Content/print.css?_' + Math.random() + '" type="text/css" media="print" />');
    windowPrint.document.write('<style type="text/css">');
    windowPrint.document.write('.noprint {display:none!important;}');
    windowPrint.document.write('table,td,th {border:1px solid black;border-collapse:collapse;}');
    windowPrint.document.write('table {border-collapse:collapse;}');
    windowPrint.document.write('</style>');
    windowPrint.document.write('</head><body >');
    windowPrint.document.write(data);
    windowPrint.document.write('</body></html>');

    windowPrint.document.close(); // necessary for IE >= 10
    windowPrint.focus(); // necessary for IE >= 10

    windowPrint.print();
    //windowPrint.open(); // for test only
    windowPrint.close();

    return true;
}

function toDateLabel(dateTime) {
    var mEpoch = parseInt(dateTime); 
    var date = new Date();

    if(mEpoch<10000000000) mEpoch *= 1000;

    date.setTime(mEpoch)

    var day = date.getDate();
    var monthIndex = date.getMonth();
    var year = date.getFullYear();

    return monthNames[monthIndex] + " " + year;
}