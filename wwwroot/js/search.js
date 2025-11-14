$(document).ready(function () {
    console.log("Script cargado ✅");

    // Ejemplo de prueba
    console.log("Filas:", $("tbody tr").length);
    console.log("Celdas:", $("tbody tr").first().find("td").length);
    // 1) Guardar HTML original SOLO para las celdas que no son la columna de Acciones
    $('tbody tr').each(function () {
        $(this).find('td:not(.acciones)').each(function () {
            $(this).attr('data-original', $(this).html());
        });
    });

    // Escapa caracteres especiales para usar en RegExp
    function escapeRegExp(str) {
        return str.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    }

    // Resalta solo en nodos de texto, sin tocar etiquetas
    function highlightTextNode(node, regex) {
        const text = node.nodeValue;
        let lastIndex = 0;
        const frag = document.createDocumentFragment();

        let match;
        // Usamos exec con un regex sin la bandera 'g' re-crear por cada llamada
        // Aquí asumimos regex ya provisto con 'i' o 'gi'
        const localRegex = new RegExp(regex.source, regex.flags);

        while ((match = localRegex.exec(text)) !== null) {
            if (match.index > lastIndex) {
                frag.appendChild(document.createTextNode(text.substring(lastIndex, match.index)));
            }
            const span = document.createElement('span');
            span.className = 'bg-warning';
            span.textContent = match[0]; // así no introducimos HTML
            frag.appendChild(span);
            lastIndex = match.index + match[0].length;
        }
        if (lastIndex < text.length) {
            frag.appendChild(document.createTextNode(text.substring(lastIndex)));
        }
        node.parentNode.replaceChild(frag, node);
    }

    function filterTable() {
        const raw = $('#searchInput').val() || '';
        const value = raw.trim().toLowerCase();

        // si no hay valor, restauramos todo y mostramos todo
        const isEmpty = value.length === 0;
        const regex = new RegExp(escapeRegExp(value), 'gi');

        $('tbody tr').each(function () {
            const $row = $(this);

            // Restaurar SOLO las celdas que guardamos (no tocamos la columna Acciones)
            $row.find('td:not(.acciones)').each(function () {
                const original = $(this).attr('data-original');
                if (typeof original !== 'undefined') {
                    $(this).html(original);
                }
            });

            // Comprobar coincidencia en texto de todas las columnas menos la de Acciones
            const rowText = $row.find('td:not(.acciones)').text().toLowerCase();
            const match = isEmpty ? true : rowText.indexOf(value) > -1;
            $row.toggle(match);

            // Si coincide y hay texto a buscar: resaltar solo nodos de texto
            if (match && !isEmpty) {
                $row.find('td:not(.acciones)').each(function () {
                    // para cada nodo hijo, solo procesamos nodos de texto
                    $(this).contents().each(function () {
                        if (this.nodeType === Node.TEXT_NODE && this.nodeValue.trim() !== '') {
                            highlightTextNode(this, regex);
                        }
                    });
                });
            }
        });
    }

    // Eventos
    $('#searchInput').on('input keyup search', filterTable);
    $('#searchButton').on('click', filterTable);
});

console.log("algo sale")
