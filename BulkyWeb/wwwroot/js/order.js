var datatable;
$(document).ready(function() {
    datatable = $('#tblData').DataTable({
        ajax: {
            url: '/admin/order/getall',
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
});