class OrderValidation {
    constructor() {
        this.form = document.querySelector('.order-form');
        this.addressInput = document.querySelector('.delivery-address textarea');
        this.submitButton = document.querySelector('.checkout-button');
        
        this.init();
    }

    init() {
        this.form.addEventListener('submit', (e) => this.handleSubmit(e));
        this.addressInput.addEventListener('input', () => this.validateAddress());
    }

    validateAddress() {
        const address = this.addressInput.value.trim();
        const errorElement = this.addressInput.parentElement.querySelector('.error-message');
        
        if (!address) {
            this.showError('Пожалуйста, введите адрес доставки');
            return false;
        }
        
        if (address.length < 10) {
            this.showError('Адрес должен содержать не менее 10 символов');
            return false;
        }

        if (errorElement) {
            errorElement.remove();
        }
        return true;
    }

    showError(message) {
        const errorElement = this.addressInput.parentElement.querySelector('.error-message');
        
        if (errorElement) {
            errorElement.textContent = message;
        } else {
            const error = document.createElement('div');
            error.className = 'error-message';
            error.textContent = message;
            this.addressInput.parentElement.appendChild(error);
        }
    }

    handleSubmit(e) {
        e.preventDefault();
        
        if (this.validateAddress()) {
            // Если валидация прошла успешно, отправляем форму
            this.form.submit();
        }
    }
}

// Инициализация валидации при загрузке страницы
document.addEventListener('DOMContentLoaded', () => {
    new OrderValidation();
}); 