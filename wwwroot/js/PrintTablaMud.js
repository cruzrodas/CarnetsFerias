window.printTable = function () {

    const printStyles = `
        @media print {
            body  { visibility: hidden; }
            .printable-table, .printable-table * { visibility: visible; }
            .printable-table { position: absolute; left: 0; top: 0; width: 100%; border-collapse: collapse; margin: 0 auto; }

            .printable-table th, .printable-table td {
                border: 1px solid #000;
                padding: 0px; 
                text-align: left;
                font-size: 11px; 
            }
            .printable-table th {
                background-color: #f2f2f2;
                font-weight: bold;
            }

            .printable-table .acciones {
                display: none;
            }


            .print-header {
                display: block;
            }

            @page {
                margin: 10mm;
               <h1>Hola</h1>
            }

        }
    `;


    const styleSheet = document.createElement("style");
    styleSheet.type = "text/css";
    styleSheet.innerHTML = printStyles;
    document.head.appendChild(styleSheet);

    window.print();

    window.onafterprint = () => document.head.removeChild(styleSheet);
};
