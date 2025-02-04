// Добавляем функцию в глобальную область видимости
window.hideSuccessModal = function() {
    const modal = document.querySelector('.success-modal');
    const backdrop = document.querySelector('.modal-backdrop');
    
    if (modal && backdrop) {
        modal.classList.remove('show');
        backdrop.remove();
        document.body.style.overflow = '';
        window.location.href = '/'; // Редирект на главную
    }
};

document.addEventListener('DOMContentLoaded', function() {
    const addressInput = document.getElementById('deliveryAddress');
    
    function validateAddress() {
        const address = addressInput.value.trim();
        if (!address) {
            setInvalid('Пожалуйста, введите адрес доставки');
            return false;
        }
        if (address.length < 10) {
            setInvalid('Адрес должен содержать не менее 10 символов');
            return false;
        }
        setValid();
        return true;
    }

    function setInvalid(message) {
        addressInput.classList.add('is-invalid');
        const feedback = addressInput.nextElementSibling;
        if (feedback) {
            feedback.textContent = message;
        }
    }

    function setValid() {
        addressInput.classList.remove('is-invalid');
    }

    // Функция для показа модального окна успеха
    function showSuccessModal() {
        const modal = document.querySelector('.success-modal');
        const backdrop = document.createElement('div');
        backdrop.className = 'modal-backdrop';
        document.body.appendChild(backdrop);
        
        modal.classList.add('show');
        backdrop.classList.add('show');
        
        document.body.style.overflow = 'hidden';
    }

    // Функция для показа модального окна с ошибкой
    function showErrorModal(message) {
        const modal = document.querySelector('.error-modal');
        const errorMessage = modal.querySelector('p');
        errorMessage.textContent = message || 'Произошла ошибка при оформлении заказа';
        
        const backdrop = document.createElement('div');
        backdrop.className = 'modal-backdrop';
        document.body.appendChild(backdrop);
        
        modal.classList.add('show');
        backdrop.classList.add('show');
        
        document.body.style.overflow = 'hidden';
    }

    // Валидация при вводе
    addressInput.addEventListener('input', validateAddress);

    // Обработчик для кнопки оформления заказа
    window.placeOrder = function() {
        if (!validateAddress()) {
            return;
        }

        const deliveryAddress = addressInput.value.trim();
        const submitButton = document.querySelector('button[onclick="placeOrder()"]');
        const originalText = submitButton.innerHTML;
        submitButton.disabled = true;
        submitButton.innerHTML = '<i class="bi bi-arrow-repeat spin"></i> Обработка...';

        fetch('/Order/PlaceOrder', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ deliveryAddress: deliveryAddress })
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                showSuccessModal();
            } else {
                showErrorModal(data.message);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showErrorModal('Произошла ошибка при оформлении заказа');
        })
        .finally(() => {
            submitButton.disabled = false;
            submitButton.innerHTML = originalText;
        });
    };

    // Обработчики для кнопок в модальных окнах
    const successModalBtn = document.querySelector('.success-modal .btn-primary');
    if (successModalBtn) {
        successModalBtn.addEventListener('click', function() {
            window.location.href = '/Order/History';
        });
    }

    const errorModalBtn = document.querySelector('.error-modal .btn-danger');
    if (errorModalBtn) {
        errorModalBtn.addEventListener('click', hideErrorModal);
    }

    // Добавляем обработчик для крестика
    const closeButton = document.querySelector('.success-modal .close-modal');
    if (closeButton) {
        closeButton.addEventListener('click', function(e) {
            e.preventDefault();
            hideSuccessModal();
        });
    }
}); 