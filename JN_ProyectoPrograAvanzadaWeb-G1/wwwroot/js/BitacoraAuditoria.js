document.addEventListener("DOMContentLoaded", function () {

    if (document.getElementById("chartDonut")) {

        var donutData = window.bitacoraData || { productos: 0, bodegas: 0 };

        new Chart(document.getElementById('chartDonut'), {
            type: 'doughnut',
            data: {
                labels: ["Productos", "Bodegas"],
                datasets: [{
                    data: [donutData.productos, donutData.bodegas],
                    backgroundColor: [
                        'rgba(54, 162, 235, 0.6)',
                        'rgba(75, 192, 75, 0.6)'
                    ]
                }]
            },
            options: {
                responsive: true
            }
        });
    }


    var exportBtn = document.getElementById('btnExportCsv');

    if (exportBtn) {
        exportBtn.addEventListener('click', function () {
            var table = document.querySelector('table');
            var rows = Array.from(table.querySelectorAll('tr'));

            var csv = rows.map(function (row) {
                var cells = Array.from(row.querySelectorAll('th, td'));
                return cells.map(function (c) {
                    var txt = c.innerText.replace(/"/g, '""');
                    if (txt.search(/,|\n|"/) >= 0) txt = '"' + txt + '"';
                    return txt;
                }).join(',');
            }).join('\n');

            var blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
            var url = URL.createObjectURL(blob);
            var a = document.createElement('a');
            a.href = url;
            a.download = 'bitacora.csv';
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            URL.revokeObjectURL(url);
        });
    }

});
