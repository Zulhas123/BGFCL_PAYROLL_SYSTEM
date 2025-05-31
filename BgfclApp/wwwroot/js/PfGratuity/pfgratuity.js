let _dataTable = undefined;
$(document).ready(function () {
    let pageLength = parseInt($('#page_length_select').val());
    attachRealTimeCalculationEvents();
    // Open modal for creating a new Provident Fund entry
    $('#add_button').on('click', function () {
        $('#provident_modal').modal('show');
        $('#providentId').val('');
        $('#operation_type').val('create');
        $('#submit_button').html('Create');
        $('#providentModalLabel').html('Setup Provident Fund');
    });

    // Initial load of Provident Fund list
    getProvidentFundlist(pageLength);

    // Submit form for create/update
    $('#submit_button').on('click', function () {
        let providentId = $('#ProvidentId').val() || 0;
        let dataObj = {
            id: providentId,
            empOpeningContribution: $('#EmpOpeningContribution').val(),
            empEndingContribution: $('#EmpEndingContribution').val(),
            companyOpeningContribution: $('#CompanyOpeningContribution').val(),
            companyEndingContribution: $('#CompanyEndingContribution').val(),
            interestRate: $('#InterestRate').val(),
            interestAsOpening: $('#InterestAsOpening').val(),
            interestAsEnding: $('#InterestAsEnding').val(),
            empSubTotal: $('#EmpSubTotal').val(),
            companySubTotal: $('#CompanySubTotal').val(),
            grandTotal: $('#GrandTotal').val(),
            totalContribution: $('#TotalContribution').val(),
            interestAsYear: $('#InterestAsYear').val(),

        };

        let operationType = $('#operation_type').val();
        let ajaxUrl = operationType === 'create' ? '/api/PFGratuity/CreateProvidentFund' : '/api/PFGratuity/UpdateProvidentFund';
        let ajaxMethod = operationType === 'create' ? 'POST' : 'PUT';

        $.ajax({
            url: ajaxUrl,
            type: ajaxMethod,
            async: false,
            data: dataObj
        }).always(function (responseObject) {
            $('.error-item').empty();

            if (responseObject.statusCode === 201) {
                // Reset form
                $('#providentForm')[0].reset();
                $('#provident_modal').modal('hide');
                showToast('Success', responseObject.responseMessage, 'success');
                getProvidentFundlist(pageLength);

                if (operationType === 'update') {
                    $('#operation_type').val('create');
                    $('#submit_button').html('Create');
                    $('#provident_modal').modal('hide');
                }
            }

            else if (responseObject.statusCode === 400 || responseObject.statusCode === 409) {
                for (let error in responseObject.errors) {
                    $(`#${error}`).empty().append(responseObject.errors[error]);
                }
                showToast('Error', responseObject.responseMessage, 'error');
            }

            else if (responseObject.statusCode === 500) {
                showToast('Error', responseObject.responseMessage, 'error');
            }
        });
    });

    // Cancel button to reload the page
    $('#cancel_button').on('click', function () {
        location.reload();
    });
});

function getProvidentFundlist(pageLength) {
    if (_dataTable !== undefined) {
        _dataTable.destroy();
        _dataTable = null;
    }


    _dataTable = $('#Provident_list_table').DataTable({
        pageLength: pageLength,
        ajax: {
            url:'/api/PFGratuity/GetProvidentFund',
            dataSrc: function (json) {
                console.log("API Response: ", json); // Debug log
                if (!json || !json.data) return []; // Ensure it does not break if data is null/undefined
                return Array.isArray(json.data) ? json.data : [json.data];
            },
            error: function (xhr, error, thrown) {
                console.error("Error loading departments:", xhr.responseText);
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
            { data: 'jobCode', className: 'text-center', orderable: false },
            { data: 'employeeName', className: 'text-center', orderable: false },
            { data: 'empOpeningContribution', className: 'text-center', orderable: false },
            { data: 'empCurrentYearContribution', className: 'text-center', orderable: false },
            { data: 'companyOpeningContribution', className: 'text-center', orderable: false },
            { data: 'companyCurrentYearContribution', className: 'text-center', orderable: false },
            { data: 'interestAsOpening', className: 'text-center', orderable: false },
            { data: 'interestAsEnding', className: 'text-center', orderable: false },
            { data: 'interestAsYear', className: 'text-center', orderable: false },
            {
                data: 'id',
                className: 'text-center align-middle', orderable: false,
                render: function (data, type, row, meta) {
                    return `
                    <button type="button" class="btn btn-sm btn-warning" onclick="onEditClicked(${data})">
                        <i class="fas fa-edit"></i>
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

            // **Fix: Remove all existing filters and extra <br> tags to prevent header height increase**
            $('#Provident_list_table thead th').find('.column-filter, br').remove();

            // Add column filters dynamically
            $('#Provident_list_table thead th').each(function (index) {
                let title = $(this).text();
                if (index !== 0 && title && title !== 'Action') {
                    $(this).append(`<br><input type="text" class="form-control form-control-sm column-filter" placeholder=" " />`);
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







let empCurrentYearContribution = 0;
let companyCurrentYearContribution = 0;

function onEditClicked(providentId) {
    console.log("providentId", providentId);
    $('#provident_modal').modal('show');
    $('#operation_type').val('edit');
    $('#submit_button').html('Save');
    $('#providentModalLabel').html('Edit Account');
    $('#remove_spin').removeClass('d-none');

    $.ajax({
        url: '/api/PFGratuity/GetProvidentFundById?id=' + encodeURIComponent(providentId),
        type: 'GET'
    })
        .done(function (response) {
            console.log("data", response);

            if (response.statusCode === 200) {
                const data = response.data;

                // Store for calculations
                empCurrentYearContribution = parseFloat(data.empCurrentYearContribution) || 0;
                companyCurrentYearContribution = parseFloat(data.companyCurrentYearContribution) || 0;

                $('#ProvidentId').val(data.id);
                $('#EmpOpeningContribution').val(data.empOpeningContribution);
                $('#EmpEndingContribution').val(data.empEndingContribution);
                $('#CompanyOpeningContribution').val(data.companyOpeningContribution);
                $('#CompanyEndingContribution').val(data.companyEndingContribution);
                $('#InterestRate').val(data.interestRate);
                $('#InterestAsOpening').val(data.interestAsOpening);
                $('#InterestAsEnding').val(data.interestAsEnding);
                $('#InterestAsYear').val(data.interestAsYear);

                recalculateAll(empCurrentYearContribution, companyCurrentYearContribution); // Optional: auto-calculate all derived fields on load
            } else if (response.statusCode === 404) {
                showToast('Error', response.responseMessage, 'error');
            } else {
                showToast('Error', 'Unexpected response from server.', 'error');
            }
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            console.error('AJAX Error:', textStatus, errorThrown);
            showToast('Error', 'Failed to fetch Provident Fund details.', 'error');
        })
        .always(function () {
            $('#remove_spin').addClass('d-none');
        });

   

}

function attachRealTimeCalculationEvents() {
    $('#EmpOpeningContribution, #CompanyOpeningContribution, #InterestRate').on('input', recalculateAll);
}

function recalculateAll() {
    const empOpening = parseFloat($('#EmpOpeningContribution').val()) || 0;
    const companyOpening = parseFloat($('#CompanyOpeningContribution').val()) || 0;
    const interestRate = parseFloat($('#InterestRate').val()) || 0;

    // Use global values from API
    const empEnding = empOpening + empCurrentYearContribution;
    $('#EmpEndingContribution').val(empEnding.toFixed(2));

    const companyEnding = companyOpening + companyCurrentYearContribution;
    $('#CompanyEndingContribution').val(companyEnding.toFixed(2));

    const totalPrincipal = empOpening + companyOpening;
    const companyInterestAmount = totalPrincipal * (interestRate / 100);
    $('#InterestAsOpening').val(companyInterestAmount.toFixed(2));

    const interestAsEnding = (empEnding + companyEnding) * (interestRate / 100);
    $('#InterestAsEnding').val(interestAsEnding.toFixed(2));

    const empSubTotal = empOpening + empCurrentYearContribution;
    $('#EmpSubTotal').val(empSubTotal.toFixed(2));

    const comSubTotal = companyOpening + companyCurrentYearContribution;
    $('#CompanySubTotal').val(comSubTotal.toFixed(2));

    const interestCurrentYear = (empCurrentYearContribution + companyCurrentYearContribution) * (interestRate / 100);
    $('#InterestAsYear').val(interestCurrentYear.toFixed(2));

    const totalContribution = empEnding + companyEnding;
    $('#TotalContribution').val(totalContribution.toFixed(2));
    const grandTotal = totalContribution + interestAsEnding;
    $('#GrandTotal').val(grandTotal.toFixed(2));
}



