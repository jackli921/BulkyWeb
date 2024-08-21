var datatable;
$(document).ready(function() {
    datatable = $('#tblData').DataTable({
        ajax: {
            url: '/admin/user/getall',
        },
        columns: [
            { data: 'name' },
            { data: 'email' },
            { data: 'phoneNumber' },
            { data: 'company.name' },
            { data: '' },
            {
                data: 'id',
                render: function(data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/admin/company/upsert?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i> Edit</a>
                    </div>`
                }
            }
        ]
    });
});