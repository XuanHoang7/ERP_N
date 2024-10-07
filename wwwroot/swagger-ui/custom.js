window.onload = function () {
    // Thêm JWT token cứng
    var token = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjMwNTZjZWI3LTY3OGEtNDc5Mi1hNDM5LTkyYWQwZjc3OThlZSIsIm5iZiI6MTcyNzU3Njc4MSwiZXhwIjoxNzI3NjYzMTgxLCJpYXQiOjE3Mjc1NzY3ODF9.egYSLwh_Y6AttnozbexFMqQvRptERH5nf6Qpjd6VZeo";
    window.ui.preauthorizeApiKey("bearer", token);

    // Thêm ApiKey cứng
    var apiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjMwNTZjZWI3LTY3OGEtNDc5Mi1hNDM5LTkyYWQwZjc3OThlZSIsIm5iZiI6MTcyNzU3Njc4MSwiZXhwIjoxNzI3NjYzMTgxLCJpYXQiOjE3Mjc1NzY3ODF9.egYSLwh_Y6AttnozbexFMqQvRptERH5nf6Qpjd6VZeo";
    window.ui.preauthorizeApiKey("ApiKey", apiKey);
};
