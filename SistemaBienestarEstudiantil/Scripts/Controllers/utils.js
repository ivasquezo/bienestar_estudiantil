function printElement(element, title)
{
    var data = $(element).html();
    var windowPrint = window.open('', title, '');
    windowPrint.document.write('<html><head><title>' + title + '</title>');
    windowPrint.document.write('<link rel="stylesheet" href="../../Content/print.css?_' + Math.random() + '" type="text/css" media="print" />');
    windowPrint.document.write('<style type="text/css">');
    windowPrint.document.write('.noprint {display:none!important;}');
    windowPrint.document.write('td,th {border-bottom:1px solid black;}');
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