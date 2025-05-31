let employeeTypes = [];
let _dataTable = undefined;
$(document).ready(function () {
    let pageLength = parseInt($('#page_length_select').val());
    loadInitialData();
    getbasicList(pageLength);
    $('#add_button').on('click', function () {
        $('#basic_modal').modal('show');
        $('#BasicId').val('');
        $('#operation_type').val('create');
        $('#submit_button').html('Create');
        $('#basicModalLabel').html('Add Basic');
    });

    $('#submit_button').on('click', function () {
        let basicId = $('#BasicId').val();
        let basicAmount = $('#BasicAmount').val();
        let gradeId = $('#GradeId').val();

        if (basicId == '') {
            basicId = 0;
        }


        let dataObj = {
            id: basicId,
            basicAmount: basicAmount,
            gradeId: gradeId,
        };
        console.log(dataObj)
        let operationType = $('#operation_type').val();

        if (operationType == 'create') {


            $.ajax({
                url: '/api/Grades/CreateBasic',
                type: 'POST',
                async: false,
                data: dataObj
            }).always(function (responseObject) {
                $('.error-item').empty();
                if (responseObject.statusCode == 201) {
                    $('#BasicAmount').val(0);
                    $('#basicForm')[0].reset();
                    $('#basic_modal').modal('hide');
                    showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    getbasicList();
                }
                if (responseObject.statusCode == 400 || responseObject.statusCode == 409) {
                    for (let error in responseObject.errors) {
                        $(`#${error}`).empty();
                        $(`#${error}`).append(responseObject.errors[error]);
                    }
                    showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                }
                if (responseObject.statusCode == 500) {
                    showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                }
            });


        }
        else {

            $.ajax({
                url: '/api/Grades/UpdateBasic',
                type: 'PUT',
                async: false,
                data: dataObj
            }).always(function (responseObject) {
                $('.error-item').empty();
                if (responseObject.statusCode == 201) {
                    $('#BasicAmount').val(0);
                    $('#basicForm')[0].reset();
                    $('#basic_modal').modal('hide');
                    showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
                    getbasicList();
                    $('#operation_type').val('create');
                    $('#submit_button').html('Create');
                }
                if (responseObject.statusCode == 400) {
                    for (let error in responseObject.errors) {
                        $(`#${error}`).empty();
                        $(`#${error}`).append(responseObject.errors[error]);
                    }
                    showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                }
                if (responseObject.statusCode == 500) {
                    showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
                }
            });
            
        }


    });
    $('#cancel_button').on('click', () => {
        location.reload();
    });
    $('#page_length_select').on('change', function () {
        pageLength = parseInt($(this).val());
        getGradeList(pageLength);
    });

});


function loadInitialData() {
    $.ajax({
        url: '/api/Grades/GetGrades',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            grades = responseObject.data;
            $('#GradeId').empty();
            $('#GradeId').append('<option value="0">select one</option>');
            $.each(grades, function (key, item) {
                if (item.employeeTypeId==1) {
                    $('#GradeId').append(`<option value=${item.id}>${item.gradeName}</option>`);
                }
               

            });
        },
        error: function (responseObject) {
        }
    });

}

function getbasicList(pageLength) {

    if (_dataTable != undefined) {
        _dataTable.destroy();
    }

    _dataTable = $('#basic_list_table').DataTable({
        pageLength: pageLength,
        ajax: {
            url: '/api/Grades/GetBasics',
            dataSrc: 'data'
        },
        columns: [
            {
                data: '',
                className: 'text-center align-middle',
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: 'gradeName', className: 'text-center', orderable: false },
            { data: 'basicAmount', className: 'text-center', orderable: false },
            {
                data: 'id', orderable: false,
                className: 'text-center align-middle',
                render: (data, type, row) => {
                    return `
                        <button type="button" class="btn btn-primary btn-sm" onclick="onEditClicked(${data})">
                            <i class="fas fa-edit"></i>
                        </button> 
                        <button type="button" class="btn btn-danger btn-sm" onclick="onRemoveClicked(${data})">
                            <i class="fas fa-trash-alt"></i>
                        </button>`;
                }
            },

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
            $('#basic_list_table thead th').find('.column-filter, br').remove();

            // Add column filters dynamically
            $('#basic_list_table thead th').each(function (index) {
                let title = $(this).text();
                if (index !== 0 && title && title !== 'Action') {
                    $(this).append(`<br><input type="text" class="form-control form-control-sm column-filter" placeholder="Search" />`);
                }
            });

            // Apply column filtering
            api.columns().every(function () {
                let that = this;
                $('input.column-filter', this.header()).on('keyup change', function () {
                    if (that.search() !== this.value) {
                        that.search(this.value).draw();
                    }
                });
            });
        }
    });

}



//function onEditClicked(gradeId) {
//    $('#edit_modal').modal('show');
//    $('#GradeId').val(gradeId);
//}

function onEditClicked(basicId) {
    // Show the modal for editing
    $('#basic_modal').modal('show');

    // Set the operation type to 'edit' and update the button text
    $('#operation_type').val('edit');
    $('#submit_button').html('Save');
    $('#basicModalLabel').html('Edit Basic');

    // Show a loading indicator or spinner if needed
    $('#remove_spin').removeClass('d-none');

    // Fetch the grade data based on the gradeId
    $.ajax({
        url: '/api/Grades/GetBasicById?id=' + basicId,
        type: 'GET',
        async: false,
    }).always(function (responseObject) {
        if (responseObject.statusCode == 200) {
            // Populate the form fields with the retrieved data
            $('#BasicId').val(responseObject.data.id);
            $('#GradeId').val(responseObject.data.gradeId);
            $('#BasicAmount').val(responseObject.data.basicAmount);


        } else if (responseObject.statusCode == 404) {
            showToast('Error', responseObject.responseMessage, 'error');
            $('#basic_modal').modal('hide');
        } else {
            showToast('Error', 'An unexpected error occurred.', 'error');
            $('#basic_modal').modal('hide');
        }
        $('#remove_spin').addClass('d-none');
    });
}

function onRemoveClicked(basicId) {
    $('#remove_modal').modal('show');
    $('#BasicId').val(basicId);
}
function onRemoveConfirmed() {
    $('#remove_spin').removeClass('d-none');
    let _basicId = $('#BasicId').val();
    $('#remove_modal').modal('hide');
    $.ajax({
        url: '/api/Grades/RemoveBasic?id=' + _basicId,
        type: 'DELETE',
        async: false,
    }).always(function (responseObject) {
        if (responseObject.statusCode == 200) {
            $('#GradeId').val('');
            $('#BasicAmount').val('');
            showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
            getbasicList();
            $('#operation_type').val('create');
            $('#submit_button').html('create');
        }
        if (responseObject.statusCode == 404 || responseObject.statusCode == 500) {
            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
        }
        $('#remove_spin').addClass('d-none');
    });
}

document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('cancel_button').addEventListener('click', function () {
        document.getElementById('gradeForm').reset();
        document.getElementById('EmployeeTypeIdError').textContent = '';
        document.getElementById('GradeNameError').textContent = '';
        document.getElementById('EmployeeTypeId').selectedIndex = 0;
        document.getElementById('EmployeeTypeId').value = '';
        document.getElementById('GradeName').value = '';
    });
});