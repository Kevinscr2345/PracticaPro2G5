// Módulo de Productos. Patrón IIFE: encapsula estado y métodos del módulo.
(() => {
    "use strict";

    const Producto = {
        tabla: null,

        init() {
            this.inicializarTabla();
            this.registrarEventos();
        },

        urls: {
            listar: "/Producto/ObtenerProductos",
            obtener: "/Producto/ObtenerProductoPorId",
            crear: "/Producto/CrearProducto",
            actualizar: "/Producto/ActualizarProducto",
            eliminar: "/Producto/EliminarProducto"
        },

        formatearMoneda(valor) {
            return new Intl.NumberFormat("es-CR", { style: "currency", currency: "CRC" }).format(valor ?? 0);
        },

        inicializarTabla() {
            this.tabla = $("#tblProductos").DataTable({
                ajax: {
                    url: this.urls.listar,
                    dataSrc: "dato"
                },
                columns: [
                    { data: "id", width: "60px" },
                    { data: "nombre" },
                    { data: "nombreCategoria", defaultContent: "" },
                    {
                        data: "precio",
                        className: "text-end",
                        render: (valor) => this.formatearMoneda(valor)
                    },
                    { data: "stock", className: "text-end" },
                    {
                        data: "id",
                        orderable: false,
                        className: "text-center",
                        width: "150px",
                        render: (id) => `
                            <button class="btn btn-sm btn-outline-primary btn-editar" data-id="${id}">Editar</button>
                            <button class="btn btn-sm btn-outline-danger btn-eliminar" data-id="${id}">Eliminar</button>`
                    }
                ],
                language: {
                    url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json"
                }
            });
        },

        registrarEventos() {
            $("#formCrearProducto").on("submit", (e) => {
                e.preventDefault();
                const form = e.currentTarget;
                if (!$(form).valid()) {
                    return;
                }
                this.guardar(this.urls.crear, $(form).serialize(), "#modalCrearProducto", form);
            });

            $("#formEditarProducto").on("submit", (e) => {
                e.preventDefault();
                const form = e.currentTarget;
                if (!$(form).valid()) {
                    return;
                }
                this.guardar(this.urls.actualizar, $(form).serialize(), "#modalEditarProducto", form);
            });

            $("#tblProductos tbody").on("click", ".btn-editar", (e) => {
                const id = $(e.currentTarget).data("id");
                this.cargarParaEditar(id);
            });

            $("#tblProductos tbody").on("click", ".btn-eliminar", (e) => {
                const id = $(e.currentTarget).data("id");
                this.confirmarEliminar(id);
            });
        },

        guardar(url, datos, modal, form) {
            $.ajax({
                url: url,
                type: "POST",
                data: datos
            }).done((respuesta) => {
                if (respuesta.esCorrecto) {
                    $(modal).modal("hide");
                    form.reset();
                    this.tabla.ajax.reload(null, false);
                    Swal.fire({ icon: "success", title: "Listo", text: respuesta.mensaje, timer: 1800, showConfirmButton: false });
                } else {
                    Swal.fire({ icon: "warning", title: "Atención", text: respuesta.mensaje });
                }
            }).fail(() => {
                Swal.fire({ icon: "error", title: "Error", text: "Ocurrió un error al procesar la solicitud." });
            });
        },

        cargarParaEditar(id) {
            $.get(this.urls.obtener, { id: id }).done((respuesta) => {
                if (!respuesta.esCorrecto) {
                    Swal.fire({ icon: "warning", title: "Atención", text: respuesta.mensaje });
                    return;
                }
                const p = respuesta.dato;
                $("#editarProductoId").val(p.id);
                $("#editarProductoNombre").val(p.nombre);
                $("#editarProductoDescripcion").val(p.descripcion);
                $("#editarProductoPrecio").val(p.precio);
                $("#editarProductoStock").val(p.stock);
                $("#editarProductoFkCategoria").val(p.fkCategoria);
                $("#modalEditarProducto").modal("show");
            }).fail(() => {
                Swal.fire({ icon: "error", title: "Error", text: "No se pudo cargar el producto." });
            });
        },

        confirmarEliminar(id) {
            Swal.fire({
                title: "¿Eliminar producto?",
                text: "Esta acción no se puede deshacer.",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "Sí, eliminar",
                cancelButtonText: "Cancelar",
                confirmButtonColor: "#d33"
            }).then((resultado) => {
                if (resultado.isConfirmed) {
                    this.eliminar(id);
                }
            });
        },

        eliminar(id) {
            const token = $("#formCrearProducto input[name='__RequestVerificationToken']").val();
            $.ajax({
                url: this.urls.eliminar,
                type: "POST",
                data: { id: id, __RequestVerificationToken: token }
            }).done((respuesta) => {
                if (respuesta.esCorrecto) {
                    this.tabla.ajax.reload(null, false);
                    Swal.fire({ icon: "success", title: "Eliminado", text: respuesta.mensaje, timer: 1800, showConfirmButton: false });
                } else {
                    Swal.fire({ icon: "warning", title: "Atención", text: respuesta.mensaje });
                }
            }).fail(() => {
                Swal.fire({ icon: "error", title: "Error", text: "No se pudo eliminar el producto." });
            });
        }
    };

    $(document).ready(() => Producto.init());
})();
