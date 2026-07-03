// Módulo de Categorías. Patrón IIFE: encapsula estado y métodos del módulo.
(() => {
    "use strict";

    const Categoria = {
        tabla: null,

        init() {
            this.inicializarTabla();
            this.registrarEventos();
        },

        // ---- URLs de las acciones del controlador ----
        urls: {
            listar: "/Categoria/ObtenerCategorias",
            obtener: "/Categoria/ObtenerCategoriaPorId",
            crear: "/Categoria/CrearCategoria",
            actualizar: "/Categoria/ActualizarCategoria",
            eliminar: "/Categoria/EliminarCategoria"
        },

        inicializarTabla() {
            this.tabla = $("#tblCategorias").DataTable({
                ajax: {
                    url: this.urls.listar,
                    dataSrc: "dato"
                },
                columns: [
                    { data: "id", width: "60px" },
                    { data: "nombre" },
                    { data: "descripcion", defaultContent: "" },
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
            // Crear
            $("#formCrearCategoria").on("submit", (e) => {
                e.preventDefault();
                const form = e.currentTarget;
                if (!$(form).valid()) {
                    return;
                }
                this.guardar(this.urls.crear, $(form).serialize(), "#modalCrearCategoria", form);
            });

            // Actualizar
            $("#formEditarCategoria").on("submit", (e) => {
                e.preventDefault();
                const form = e.currentTarget;
                if (!$(form).valid()) {
                    return;
                }
                this.guardar(this.urls.actualizar, $(form).serialize(), "#modalEditarCategoria", form);
            });

            // Abrir modal de edición (evento delegado)
            $("#tblCategorias tbody").on("click", ".btn-editar", (e) => {
                const id = $(e.currentTarget).data("id");
                this.cargarParaEditar(id);
            });

            // Eliminar (evento delegado)
            $("#tblCategorias tbody").on("click", ".btn-eliminar", (e) => {
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
                const c = respuesta.dato;
                $("#editarCategoriaId").val(c.id);
                $("#editarCategoriaNombre").val(c.nombre);
                $("#editarCategoriaDescripcion").val(c.descripcion);
                $("#modalEditarCategoria").modal("show");
            }).fail(() => {
                Swal.fire({ icon: "error", title: "Error", text: "No se pudo cargar la categoría." });
            });
        },

        confirmarEliminar(id) {
            Swal.fire({
                title: "¿Eliminar categoría?",
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
            const token = $("#formCrearCategoria input[name='__RequestVerificationToken']").val();
            $.ajax({
                url: this.urls.eliminar,
                type: "POST",
                data: { id: id, __RequestVerificationToken: token }
            }).done((respuesta) => {
                if (respuesta.esCorrecto) {
                    this.tabla.ajax.reload(null, false);
                    Swal.fire({ icon: "success", title: "Eliminada", text: respuesta.mensaje, timer: 1800, showConfirmButton: false });
                } else {
                    Swal.fire({ icon: "warning", title: "Atención", text: respuesta.mensaje });
                }
            }).fail(() => {
                Swal.fire({ icon: "error", title: "Error", text: "No se pudo eliminar la categoría." });
            });
        }
    };

    $(document).ready(() => Categoria.init());
})();
