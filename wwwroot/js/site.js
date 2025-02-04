// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Проверяем наличие токена при загрузке страницы
window.onload = function() {
    const token = localStorage.getItem('jwt_token');
    const loginItem = document.getElementById('loginItem');
    const profileItem = document.getElementById('profileItem');
    const userBalance = document.getElementById('userBalance');

    if (token) {
        if (loginItem) loginItem.style.display = 'none';
        if (profileItem) profileItem.style.display = 'block';
        
        const payload = JSON.parse(atob(token.split('.')[1]));
        const userName = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
        const userNameElement = document.getElementById('userName');
        if (userNameElement) userNameElement.textContent = userName;
    } else {
        if (loginItem) loginItem.style.display = 'block';
        if (profileItem) profileItem.style.display = 'none';
    }

    if (userBalance) {
        updateUserBalance();
    }
    updateCartCount();
}

function updateCartCount() {
    fetch('/Cart/GetCartCount', {
        headers: {
            'Accept': 'application/json'
        }
    })
    .then(response => response.json())
    .then(data => {
        const cartCountElement = document.getElementById('cartItemCount');
        if (cartCountElement) {
            cartCountElement.textContent = data.count || 0;
        }
    })
    .catch(error => {
        console.error('Error updating cart count:', error);
    });
}

function updateUserBalance() {
    fetch('/Balance/GetBalance')
        .then(response => response.json())
        .then(data => {
            const balanceElement = document.getElementById('userBalance');
            if (balanceElement) {
                balanceElement.textContent = data.balance.toLocaleString('ru-RU');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showNotification('Ошибка при обновлении баланса', 'error');
        });
}

function logout() {
    fetch('/api/Auth/logout', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    })
    .then(response => {
        showNotification('Вы успешно вышли из системы');
        localStorage.removeItem('jwt_token');
        window.location.href = '/Home/Login';
    })
    .catch(error => {
        console.error('Error during logout:', error);
        showNotification('Произошла ошибка при выходе из системы', 'error');
        localStorage.removeItem('jwt_token');
        window.location.href = '/Home/Login';
    });
}

function updateQuantity(productId, delta) {
    const token = localStorage.getItem('jwt_token');
    fetch('/Cart/UpdateQuantity', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            'Authorization': token ? `Bearer ${token}` : ''
        },
        body: JSON.stringify({ 
            productId: productId, 
            delta: delta 
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            // Обновляем страницу для отображения новых данных
            window.location.reload();
        }
    })
    .catch(error => {
        console.error('Error:', error);
        alert('Ошибка при обновлении количества товара');
    });
}

let searchTimeout;

document.addEventListener('DOMContentLoaded', function() {
    const searchInput = document.querySelector('input[name="query"]');
    if (searchInput) {
        searchInput.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                if (this.value.length >= 3) {
                    this.closest('form').submit();
                }
            }, 500);
        });
    }
});

function showNotification(message, icon = 'success') {
    const Toast = Swal.mixin({
        toast: true,
        position: 'bottom-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer)
            toast.addEventListener('mouseleave', Swal.resumeTimer)
        }
    });

    Toast.fire({
        icon: icon,
        title: message
    });
}
