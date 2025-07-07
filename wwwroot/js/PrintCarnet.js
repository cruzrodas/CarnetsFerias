// PrintCarnet.js - Funciones para imprimir carnets de feria
console.log("PrintCarnet.js ESTÁ SIENDO CARGADO");

window.printCarnet = {
    // Función principal para imprimir el carnet
    printCustom: () => {
        try {
            // Obtener el contenido del carnet
            const carnetElement = document.getElementById('carnet');

            if (!carnetElement) {
                alert('Error: No se encontró el elemento carnet para imprimir');
                return false;
            }

            // Crear una nueva ventana
            const printWindow = window.open('', '', 'height=600,width=800');

            if (!printWindow) {
                alert('Error: No se pudo abrir la ventana de impresión. Verifique que su navegador permita ventanas emergentes.');
                return false;
            }

            // Crear los estilos CSS optimizados para tarjeta PVC horizontal
            const carnetStyles = `
                <style>
                    @page {
                        size: 85.60mm 53.98mm landscape; /* Tamaño tarjeta PVC estándar horizontal */
                        margin: 0;
                    }
                    * {
                        box-sizing: border-box;
                    }
                    body {
                        margin: 0;
                        padding: 0;
                        font-family: Arial, sans-serif;
                        background: white;
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        min-height: 100vh;
                    }
                    #carnet {
                        width: 85.60mm;
                        height: 53.98mm;
                        border-radius: 3.18mm; /* 5% de bordes redondeados para tarjeta PVC */
                        padding: 4mm;
                        background-image: url('/Imagen/fondo.png');
                        background-size: cover;
                        background-position: center;
                        background-repeat: no-repeat;
                        border: 1px solid #000;
                        position: relative;
                        overflow: hidden;
                        display: flex;
                        flex-direction: column;
                    }
                    .contenido {
                        display: flex;
                        align-items: flex-start;
                        margin-bottom: 2mm;
                    }
                    .img {
                        width: 20mm !important;
                        height: 20mm !important;
                        border: 1mm solid #3a0040 !important;
                        border-radius: 2mm;
                        object-fit: cover;
                        margin-right: 3mm;
                        flex-shrink: 0;
                    }
                    .texto-azul2 {
                        color: #3a0040 !important;
                        font-size: 3mm !important;
                        font-weight: bold !important;
                        text-align: center !important;
                        margin: 1mm 0 !important;
                        line-height: 1.1;
                        -webkit-print-color-adjust: exact;
                        print-color-adjust: exact;
                    }
                    .columnas {
                        display: flex;
                        justify-content: space-between;
                        width: 100%;
                        flex: 1;
                        align-items: flex-start;
                    }
                    .columna {
                        flex: 1;
                        padding: 1mm;
                    }
                    .text {
                        font-size: 4mm !important;
                        font-weight: bold !important;
                        color: #000 !important;
                        margin: 0.5mm 0 !important;
                        line-height: 1.1;
                    }
                    .texto-azul {
                        color: #3a0040 !important;
                        font-size: 2.5mm !important;
                        font-weight: bold !important;
                        -webkit-print-color-adjust: exact;
                        print-color-adjust: exact;
                    }
                    .area {
                        font-size: 2.8mm !important;
                        font-weight: bold !important;
                        color: #000 !important;
                        margin: 0.5mm 0 !important;
                    }
                    #carnet p {
                        font-size: 2.2mm !important;
                        margin: 0.5mm 0 !important;
                        line-height: 1.2;
                    }
                    
                    /* Estilos específicos para impresión */
                    @media print {
                        body {
                            margin: 0 !important;
                            padding: 0 !important;
                            background: white !important;
                        }
                        #carnet {
                            page-break-inside: avoid;
                            border: 1px solid #000 !important;
                        }
                        .texto-azul, .texto-azul2 {
                            color: #3a0040 !important;
                            -webkit-print-color-adjust: exact !important;
                            print-color-adjust: exact !important;
                        }
                    }
                </style>
            `;

            // Escribir el contenido HTML completo
            printWindow.document.write('<html><head><title>Carnet de Feria</title>');
            printWindow.document.write('<meta charset="utf-8">');
            printWindow.document.write('<meta name="viewport" content="width=device-width, initial-scale=1.0">');
            printWindow.document.write(carnetStyles);
            printWindow.document.write('</head><body>');
            printWindow.document.write(carnetElement.outerHTML);
            printWindow.document.write('</body></html>');
            printWindow.document.close(); // Cierra el documento
            printWindow.print(); // Abre el cuadro de diálogo de impresión

            return true;

        } catch (error) {
            console.error('Error en printCustom:', error);
            alert('Error al procesar la impresión: ' + error.message);
            return false;
        }
    }
};

// Función de compatibilidad (para mantener código existente)
window.printCustom = window.printCarnet.printCustom;