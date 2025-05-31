let _dataTable = undefined;
$(document).ready(function () {
    let pageLength = parseInt($('#page_length_select').val()) || 10;

    getSchoolList(pageLength);

    $('#page_length_select').on('change', function () {
        pageLength = parseInt($(this).val());
        getSchoolList(pageLength);
    });

    $('#filter_select').select2();

    $('#add_button').on('click', function () {
        $('#school_modal').modal('show');
        $('#schoolformId')[0].reset();
        $('#operation_type').val('create');
        $('#submit_button').html('Create');
        $('#schoolModalLabel').html('Add School');
    });

    $('#submit_button').on('click', function () {
        let schoolId = parseInt($('#SchoolId').val()) || 0;
        let title = $('#Title').val().trim();
        let notes = $('#Notes').val().trim();
        let isActive = $('#Is_Active').is(":checked");
        let operationType = $('#operation_type').val();

        let dataObj = {
            id: schoolId,
            short_id: 0,
            userId: 0,
            guestPkId: 0,
            has_erp: $('#has_erp').is(":checked"),
            has_payroll: $('#has_payroll').is(":checked"),
            title: title,
            notes: notes,
            isActive: isActive
        };

        let url = operationType === 'create' ? '/api/Schools/CreateSchool' : '/api/Schools/UpdateSchool';
        let method = operationType === 'create' ? 'POST' : 'PUT';

        $.ajax({
            url: url,
            type: method,
            contentType: 'application/json',
            data: JSON.stringify(dataObj),
            success: function (response) {
                if (response.statusCode === 200 || response.statusCode === 201) {
                    $('#school_modal').modal('hide');
                    showToast('Success', response.responseMessage, 'success');
                    getSchoolList(pageLength);
                } else {
                    showToast('Error', response.responseMessage, 'error');
                }
            },
            error: function (xhr) {
                let errorMessage = "An error occurred. Please try again.";
                if (xhr.responseJSON && xhr.responseJSON.responseMessage) {
                    errorMessage = xhr.responseJSON.responseMessage;
                }
                showToast('Error', errorMessage, 'error');
                console.error("Error response:", xhr.responseText);
            }
        });
    });

    $('#cancel_button').on('click', function () {
        $('#schoolformId')[0].reset();
        $('#school_modal').modal('hide');
    });
});

function getSchoolList(pageLength) {
    if (_dataTable) {
        _dataTable.destroy();
        $('#school_list_table tbody').empty(); // Clear only tbody, keeping headers
    }

    const url = '/api/Schools/GetSchools';

    _dataTable = $('#school_list_table').DataTable({
        pageLength: pageLength,
        destroy: true,
        ajax: {
            url: url,
            dataSrc: function (json) {
                console.log("Received JSON from API:", json);
                return json && json.data ? (Array.isArray(json.data) ? json.data : [json.data]) : [];
            },
            error: function (xhr) {
                console.error("Error loading schools:", xhr.responseText);
            }
        },
        columns: [
            {
                data: null,
                className: 'text-center align-middle',
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: 'title', className: 'text-center', orderable: false },
            {
                data: 'isActive',orderable: false,
                className: 'text-center',
                render: function (data) {
                    return data ? '<span class="text-success">Active</span>' : '<span class="text-danger">Inactive</span>';
                }
            },
            {
                data: 'id',
                className: 'text-center align-middle',orderable: false,
                render: function (data) {
                    return `<button type="button" class="btn btn-primary btn-sm" onclick="onEditClicked(${data})">
                                <i class="fas fa-edit"></i>
                            </button> 
                            <button type="button" class="btn btn-danger btn-sm" onclick="onRemoveClicked(${data})">
                                <i class="fas fa-trash"></i>
                            </button>`;
                }
            }
        ],
        pageLength: 20,
        lengthMenu: [[10, 20, 50, 100, -1], [10, 20, 50, 100, 'All']],
        lengthChange: true,
        dom: '<"d-flex justify-content-between align-items-center mb-2"lfB>t<"d-flex justify-content-between align-items-center mt-2"ip>',
        buttons: ['copy', 'excel', 'pdf', 'print'],
        autoWidth: false,
        language: {
            lengthMenu: 'Data per page _MENU_  '
        },
        initComplete: function () {
            let api = this.api();
            $('#school_list_table thead th .column-filter').remove();
            $('#school_list_table thead th').each(function (index) {
                let title = $(this).text();
                if (index !== 0 && title && title !== 'Action') {
                    $(this).append(`<br><input type="text" class="form-control form-control-sm column-filter" placeholder=" " />`);
                }
            });

            api.columns().every(function () {
                let that = this;
                $('input.column-filter', this.header()).on('keyup change', function () {
                    if (that.search() !== this.value) {
                        that.search(this.value).draw(false);
                    }
                });
            });
        }
    });
}

function onEditClicked(schoolId) {
    $('#school_modal').modal('show');
    $('#schoolformId')[0].reset();
    $('#operation_type').val('edit');
    $('#submit_button').html('Save');
    $('#schoolModalLabel').html('Edit School');

    $.ajax({
        url: `/api/Schools/GetSchoolById?id=${schoolId}`,
        type: 'GET',
        success: function (response) {
            if (response.statusCode === 200 && response.data) {
                $('#SchoolId').val(response.data.id);
                $('#Title').val(response.data.title);
                $('#Notes').val(response.data.notes);
                $('#Is_Active').prop('checked', response.data.isActive);
                $('#has_erp').prop('checked', response.data.has_erp);
                $('#has_payroll').prop('checked', response.data.has_payroll);
            } else {
                showToast('Error', response.responseMessage || 'School not found.', 'error');
                $('#school_modal').modal('hide');
            }
        },
        error: function () {
            showToast('Error', 'An error occurred while fetching school details.', 'error');
            $('#school_modal').modal('hide');
        }
    });
}

function onRemoveClicked(schoolId) {
    $('#remove_modal').modal('show');
    $('#SchoolId').val(schoolId);
}

function onRemoveConfirmed() {
    let schoolId = $('#SchoolId').val();
    $('#remove_modal').modal('hide');

    $.ajax({
        url: `/api/Schools/DeleteSchool?id=${schoolId}`,
        type: 'DELETE',
        success: function (response) {
            if (response.statusCode === 200) {
                $('#school_modal').modal('hide');
                showToast('Success', response.responseMessage, 'success');
                getSchoolList(pageLength);
            } else {
                showToast('Error', response.responseMessage, 'error');
            }
        },
        error: function () {
            showToast('Error', 'An error occurred while deleting the role.', 'error');
        }
    });
}
