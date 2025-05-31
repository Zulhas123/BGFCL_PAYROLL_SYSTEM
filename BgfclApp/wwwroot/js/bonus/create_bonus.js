let _dataTable = undefined;
$(document).ready(function () {
    loadInitialData();

    let pageLength = parseInt($('#page_length_select').val());
    getBonusList(pageLength);

    $('#EmployeeTypeId').on('change', function () {
        const selectedText = $("#EmployeeTypeId option:selected").text().toLowerCase();

        let bonusId = +$('#BonusID').val(); // Ensure it's a number
        let employeeTypeId = +$('#EmployeeTypeId').val();
        //let selectedText = $('#EmployeeTypeId option:selected').text().toLowerCase();
        console.log("bonusId", bonusId, "employeeTypeId", employeeTypeId)
        if (bonusId === 4 && employeeTypeId === 1) {
            // Special case: bonusId 4 and employeeType 1
            $('#bonusValueLabel').text('Fixed Amount');
            $('#bonusValueInput').attr('placeholder', 'Enter fixed amount');
            $('#bonusValueGroup').show();
        } else if (selectedText.includes('permanent')) {
            $('#bonusValueLabel').text('% of Basic Salary');
            $('#bonusValueInput').attr('placeholder', 'Enter percentage');
            $('#bonusValueGroup').show();
        } else if (selectedText.includes('contract')) {
            //$('#bonusValueLabel').text('Fixed Amount');
            //$('#bonusValueInput').attr('placeholder', 'Enter fixed amount');
            //$('#bonusValueGroup').show();
        } else {
            $('#bonusValueGroup').hide();
            $('#bonusValueInput').val('');
        }

    });
    $('#cancel_button').on('click', () => {
        $('#bonusForm')[0].reset(); // ✅ reset the form
        $('#bonusModalLabel').text('');
        $('#bonusModal').modal('hide');
    });

    $('#page_length_select').on('change', function () {
        pageLength = parseInt($(this).val());
        getBonusList(pageLength);
    });
    $('#add_button').on('click', function () {
        console.log("Add Button Clicked");
        $('#bonus_modal_create').modal('show');
        $('#BonusId').val('');
        $('#operation_type').val('create');
        $('#submit_button').html('Create');
        $('#bonusModalLabel').html('Add Bonus');
    });

    $('#submit_button').on('click', function () {
        let _monthId = getMonthId();
        let _bonusTitle = $('#BonusTitle').val();
        let bonusId = $('#BonusId').val();
        let isActive = $('#IsActive').is(":checked");
        let isFestival = $('#IsFestival').is(":checked");
        let isIncentive = $('#IsIncentive').is(":checked");
        let isHonorarium = $('#IsHonorarium').is(":checked");
        let isScholarship = $('#IsScholarship').is(":checked");
        if (bonusId == '') {
            bonusId = 0;
        }

        let _obj = {
            id: bonusId,
            userId: 1,
            schoolId: 1,
            roleId: 1,
            guestPkId: 1,
            bonusTitle: _bonusTitle,
            payableMonth: _monthId,
            statusOF: 1,
            statusJS: 1,
            isActive: true,
            isFestival: isFestival,
            isIncentive: isIncentive,
            isHonorarium: isHonorarium,
            isScholarship: isScholarship,
        };

        let operationType = $('#operation_type').val();
        let apiUrl = operationType === 'create' ? '/api/Bonus/CreateBonus' : '/api/Bonus/UpdateBonus';
        let requestType = operationType === 'create' ? 'POST' : 'PUT';

        $.ajax({
            url: apiUrl,
            type: requestType,
            async: false,
            data: _obj,
            success: function (responseObject) {
                if (responseObject.statusCode == 201) {
                    $('#BonusTitle').val('');
                    $('#bonus_modal_create').modal('hide');
                    showToast('Success', responseObject.responseMessage, 'success');
                    getBonusList();
                } else {
                    showToast('Error', responseObject.responseMessage, 'error');
                }
            },
            error: function () {
                showToast('Error', 'Something went wrong', 'error');
            }
        });
    });

    $('#cancel_button').on('click', () => {
        location.reload();
    });
;
    $('#process_cancel_button').on('click', () => {
        $('#bonusForm')[0].reset();
        location.reload();// ✅ reset the form
        $('#bonusModalLabel').text('');
        $('#bonusModal').modal('hide');
    });
});

function getMonthId() {
    let month = $('#months').val();
    let year = $('#years').val();

    return (parseInt(year) * 100) + parseInt(month);
}
$('#process_btn').on('click', () => {
    console.log("Click");
    $('.error-item').text('');

    const monthId = getMonthId();
    console.log("MonthId:", monthId);

    const dataObj = {
        SchoolId: +$('#schoolId').val(),
        GuestPkId: +$('#guestPkId').val(),
        RoleId: +$('#roleId').val(),
        UserId: +$('#userId').val(),
        MonthId: monthId,
        Month: +$('#Months').val(),
        Year: +$('#Years').val(),
        bonusID: +$('#BonusID').val(),
        EmployeeTypeId: +$('#EmployeeTypeId').val(),
        FestivalBonus: $('#FestivalBonus').val() ? +$('#FestivalBonus').val() : null
    };
    console.log("dataObj:", dataObj);
    let hasError = false;
    if (!dataObj.bonusID) {
        $('#BonusIDError').text('Select Bonus');
        hasError = true;
    }
    if (!dataObj.EmployeeTypeId) {
        $('#EmployeeTypeIdError').text('Select Employee Type');
        hasError = true;
    }
    if (hasError) return;

    console.log("dataObj-1:", dataObj); // This will now run

    $.ajax({
        url: '/api/Bonus/ProcessBonusData',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(dataObj),
        success: resp => {
            const type = resp.statusCode === 201 ? 'success' : 'error';
            $('#bonusModal').modal('hide');
            showToast(type === 'success' ? 'Success' : 'Error', resp.responseMessage, type);
        },
        error: () => showToast('Error', 'Failed to process bonus data.', 'error')
    });
});
function loadInitialData() {

    $.ajax({
        url: '/api/Others/GetMonths',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            let months = responseObject.data;
            let currentMonth = new Date().getMonth() + 1;
            $('#months').empty();
            $.each(months, function (key, item) {
                if (currentMonth == parseInt(item.monthId)) {
                    $('#months,#Months').append(`<option value=${item.monthId} selected>${item.monthName}</option>`);
                }
                else {
                    $('#months,#Months').append(`<option value=${item.monthId}>${item.monthName}</option>`);
                }

            });
        },
        error: function (responseObject) {
        }
    });

    $.ajax({
        url: '/api/Others/GetYears',
        type: 'Get',
        async: false,
        dataType: 'json',
        success: function (responseObject) {
            let years = responseObject.data;
            let currentYear = new Date().getFullYear();
            $('#years').empty();
            $.each(years, function (key, item) {
                if (item == currentYear) {
                    $('#years,#Years').append(`<option value=${item} selected>${item}</option>`);
                }
                else {
                    $('#years,#Years').append(`<option value=${item}>${item}</option>`);
                }
            });
        },
        error: function (responseObject) {
        }
    });
    $.ajax({
        url: '/api/Employees/GetEmployeeTypes',
        type: 'GET',
        dataType: 'json',
        success: function (responseObject) {
            const employeeTypes = responseObject.data;
            const $employeeTypeSelect = $('#EmployeeTypeId');
            $employeeTypeSelect.empty();
            $employeeTypeSelect.append(`<option value="0">Select One</option>`);
            $.each(employeeTypes, function (key, item) {
                $employeeTypeSelect.append(`<option value="${item.id}">${item.employeeTypeName}</option>`);
            });
        },
        error: function () {
            alert('Failed to load employee types.');
        }
    });
    $.ajax({
        url: '/api/Bonus/GetBonus',
        type: 'GET',
        dataType: 'json',
        success: function (responseObject) {
            const bonuses = responseObject.data;
            const $bonusSelect = $('#BonusId');
            $bonusSelect.empty();
            $bonusSelect.append(`<option value="0">Select Bonus</option>`);
            $.each(bonuses, function (key, item) {
                //$bonusSelect.append(`<option value="${item.id}">${item.bonusTitle}</option>`);
                $('#BonusId,#BonusID').append(`<option value="${item.id}">${item.bonusTitle}</option>`);
            });
        },
        error: function () {
            alert('Failed to load bonuses.');
        }
    });

}

function getBonusList() {
    if (_dataTable !== undefined) {
        _dataTable.destroy();
    }

    _dataTable = $('#bonus_list_table').DataTable({
        ajax: {
            url: '/api/Bonus/GetBonus',
            dataSrc: 'data'
        },
        columns: [
            {
                data: null,
                className: 'text-center',
                render: (data, type, row, meta) => meta.row + 1 // Auto index
            },
            { data: 'bonusTitle', className: 'text-center', orderable: false },
            {
                data: 'payableMonth',
                orderable: false,
                className: 'text-center',
                render: function (data) {
                    if (!data) return ''; // Handle null or empty values

                    let year = data.toString().substring(0, 4); // Extract year (YYYY)
                    let month = data.toString().substring(4, 6); // Extract month (MM)

                    // Convert numeric month to full month name
                    let monthNames = [
                        "January", "February", "March", "April", "May", "June",
                        "July", "August", "September", "October", "November", "December"
                    ];
                    let monthName = monthNames[parseInt(month, 10) - 1]; // Convert MM to month name

                    return `${monthName}-${year}`;
                }
            },

            {
                data: 'isActive',
                className: 'text-center',
                orderable: false,
                render: data => data ? '<span class="text-success">Active</span>' : '<span class="text-danger">Inactive</span>'
            },
            {
                data: 'id',
                className: 'text-center',
                render: data => `
                    <button type="button" class="btn btn-primary btn-sm" onclick="onEditClicked(${data})">
                        <i class="fas fa-edit"></i>
                    </button> 
                    <button type="button" class="btn btn-danger btn-sm" onclick="onRemoveClicked(${data})">
                        <i class="fas fa-trash"></i>
                  <button type="button" class="btn btn-info btn-sm" onclick="Onprocess(${data})">
                  <i class="fas fa-gift"></i> Process
                </button>

                `

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

            // Remove old filters before appending new ones
            $('#bonus_list_table thead th input.column-filter').remove();

            // Add filters dynamically
            $('#bonus_list_table thead th').each(function (index) {
                let title = $(this).text().trim();
                if (index !== 0 && title && title !== 'Action') {
                    $(this).append(`<input type="text" class="form-control form-control-sm column-filter" placeholder="Search" />`);
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

function Onprocess(bonusId) {
    console.log("bonusId", bonusId);

    // Show the modal
    $('#bonusModal').modal('show');

    // Load Bonus dropdown
    $.ajax({
        url: '/api/Bonus/GetBonus',
        type: 'GET',
        dataType: 'json',
        success: function (responseObject) {
            const bonuses = responseObject.data;
            const $bonusSelect = $('#BonusID');
            $bonusSelect.empty();
            $bonusSelect.append(`<option value="0">Select Bonus</option>`);
            $.each(bonuses, function (key, item) {
                $bonusSelect.append(`<option value="${item.id}">${item.bonusTitle}</option>`);
            });

            // Set selected bonusId if provided
            if (bonusId && bonusId > 0) {
                $bonusSelect.val(bonusId);
            }
        },
        error: function () {
            alert('Failed to load bonuses.');
        }
    });

    // Load Employee Type dropdown
    $.ajax({
        url: '/api/Employees/GetEmployeeTypes',
        type: 'GET',
        dataType: 'json',
        success: function (responseObject) {
            const employeeTypes = responseObject.data;
            const $employeeTypeSelect = $('#EmployeeTypeId');
            $employeeTypeSelect.empty();
            $employeeTypeSelect.append(`<option value="0">Select One</option>`);
            $.each(employeeTypes, function (key, item) {
                $employeeTypeSelect.append(`<option value="${item.id}">${item.employeeTypeName}</option>`);
            });
        },
        error: function () {
            alert('Failed to load employee types.');
        }
    });
}


function onRemoveClicked(bonusId) {
    $('#remove_modal').modal('show');
    $('#BonusId').val(bonusId);
}

function onRemoveConfirmed() {
    $('#remove_spin').removeClass('d-none');
    let _bonusId = $('#BonusId').val();
    $('#remove_modal').modal('hide');
    $.ajax({
        url: '/api/Bonus/RemoveBonus?id=' + _bonusId,
        type: 'DELETE',
        async: false,
    }).always(function (responseObject) {
        if (responseObject.statusCode == 200) {
            getBonusList();
            showToast(title = 'Success', message = responseObject.responseMessage, toastrType = 'success');
        }
        if (responseObject.statusCode == 404 || responseObject.statusCode == 500) {
            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
        }
        $('#remove_spin').addClass('d-none');
    });
}

//function onEditClicked(bonusId) {
//    $('#edit_modal').modal('show');
//    $('#BonusId').val(bonusId);
//}

function onEditClicked(bonusId) {
    // Show the modal for editing
    $('#bonus_modal_create').modal('show');

    // Set the operation type to 'edit' and update the button text
    $('#operation_type').val('edit');
    $('#submit_button').html('Save');
    $('#bonusModalLabel').html('Edit Bonus');
    $('#remove_spin').removeClass('d-none');

    // Fetch the department data based on the departmentId
    $.ajax({
        url: '/api/Bonus/GetBonusById?id=' + bonusId,
        type: 'GET',
        async: false,
    }).always(function (responseObject) {
        console.log("resp", responseObject.data)
        if (responseObject.statusCode == 200) {
            $('#BonusId').val(responseObject.data.id);
            $('#BonusTitle').val(responseObject.data.bonusTitle);
            $('#IsActive').prop('checked', responseObject.data.isActive);
            let payableMonth = parseInt(responseObject.data.payableMonth);
            let year = Math.trunc(payableMonth / 100);
            let month = payableMonth % 100;


            $.ajax({
                url: '/api/Others/GetMonths',
                type: 'Get',
                async: false,
                dataType: 'json',
                success: function (responseObject) {
                    let months = responseObject.data;
                    $('#months').empty();
                    $.each(months, function (key, item) {
                        if (month == parseInt(item.monthId)) {
                            $('#months').append(`<option value=${item.monthId} selected>${item.monthName}</option>`);
                        }
                        else {
                            $('#months').append(`<option value=${item.monthId}>${item.monthName}</option>`);
                        }

                    });
                },
                error: function (responseObject) {
                }
            });

            $.ajax({
                url: '/api/Others/GetYears',
                type: 'Get',
                async: false,
                dataType: 'json',
                success: function (responseObject) {
                    let years = responseObject.data;
                    $('#years').empty();
                    $.each(years, function (key, item) {
                        if (item == year) {
                            $('#years').append(`<option value=${item} selected>${item}</option>`);
                        }
                        else {
                            $('#years').append(`<option value=${item}>${item}</option>`);
                        }
                    });
                },
                error: function (responseObject) {
                }
            });



        }
        if (responseObject.statusCode == 404 || responseObject.statusCode == 500) {
            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
        }
        $('#remove_spin').addClass('d-none');
    });
}



//function onEditConfirmed() {
//    $('#remove_spin').removeClass('d-none');
//    let _bonusId = $('#BonusId').val();
//    $('#operation_type').val('edit');
//    $('#submit_button').html('Save');
//    $('#edit_modal').modal('hide');
//    $('.cancel_button_show').css('display', 'inline-block');
//    $.ajax({
//        url: '/api/Bonus/GetBonusById?id=' + _bonusId,
//        type: 'GET',
//        async: false,
//    }).always(function (responseObject) {
//        if (responseObject.statusCode == 200) {
//            $('#BonusTitle').val(responseObject.data.bonusTitle);
//            let payableMonth = parseInt(responseObject.data.payableMonth);
//            let year = Math.trunc(payableMonth / 100);
//            let month = payableMonth % 100;


//            $.ajax({
//                url: '/api/Others/GetMonths',
//                type: 'Get',
//                async: false,
//                dataType: 'json',
//                success: function (responseObject) {
//                    let months = responseObject.data;
//                    $('#months').empty();
//                    $.each(months, function (key, item) {
//                        if (month == parseInt(item.monthId)) {
//                            $('#months').append(`<option value=${item.monthId} selected>${item.monthName}</option>`);
//                        }
//                        else {
//                            $('#months').append(`<option value=${item.monthId}>${item.monthName}</option>`);
//                        }

//                    });
//                },
//                error: function (responseObject) {
//                }
//            });

//            $.ajax({
//                url: '/api/Others/GetYears',
//                type: 'Get',
//                async: false,
//                dataType: 'json',
//                success: function (responseObject) {
//                    let years = responseObject.data;
//                    $('#years').empty();
//                    $.each(years, function (key, item) {
//                        if (item == year) {
//                            $('#years').append(`<option value=${item} selected>${item}</option>`);
//                        }
//                        else {
//                            $('#years').append(`<option value=${item}>${item}</option>`);
//                        }
//                    });
//                },
//                error: function (responseObject) {
//                }
//            });



//        }
//        if (responseObject.statusCode == 404 || responseObject.statusCode == 500) {
//            showToast(title = 'Error', message = responseObject.responseMessage, toastrType = 'error');
//        }
//        $('#remove_spin').addClass('d-none');
//    });
//}
