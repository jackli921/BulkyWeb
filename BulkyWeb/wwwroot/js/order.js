var datatable;
$(document).ready(function() {
    var url = window.location.search
    var status = "all"; // default value

    if (url.includes("inprocess")) {
        status = "inprocess";
    } else if (url.includes("completed")) {
        status = "completed";
    } else if (url.includes("pending")) {
        status = "pending";
    } else if (url.includes("approved")) {
        status = "approved";
    }
    loadDataTable(status);
});

function loadDataTable(status){
    datatable = $('#tblData').DataTable({
        ajax: {
            url: `/admin/order/getall?status=${status}`,
        },
        columns: [
            { data: 'id' },
            { data: 'name' },
            { data: 'phoneNumber' },
            { data: 'applicationUser.email' },
            { data: 'orderStatus' },
            { data: 'orderTotal' },
            {
                data: 'id',
                render: function(data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/admin/order/details?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i> Edit</a>
                    </div>`
                }
            }
        ]
    });
}