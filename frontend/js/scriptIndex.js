function togglePassword() {
    const passwordField = document.getElementById('password');
    const toggleButton = document.querySelector('.toggle-password');
    if (passwordField.type === 'password') {
        passwordField.type = 'text';
        toggleButton.textContent = '🙈';
    } else {
        passwordField.type = 'password';
        toggleButton.textContent = '👁️';
    }
}

function register(){
    window.location.href = 'register.html';
}
window.register = register;

document.getElementById('loginForm').addEventListener('submit', async function (event) {
    event.preventDefault();
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    try {
        const response = await fetch('http://192.168.0.99:5203/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ email, password }),
        });

        if (!response.ok) {
            const errorData = await response.json();
            console.log(errorData.errors);
            let erro;
            if(errorData.errors){
                erro = Object.keys(errorData.errors)[0];
            }
            document.getElementById('errorMessage').innerText = errorData.error || errorData.errors[erro][0] || 'Erro desconhecido';
            $('#errorModal').modal('show');
            return;
        }

        const data = await response.json();
        localStorage.setItem('token', data.token);
        console.log(data);
        if(data.user.role == 'Admin'){
            window.location.href = 'paginaInicial.html?param=1';
        }
        else{
            window.location.href = 'paginaInicial.html?param=2';
        }
    } catch (error) {
        document.getElementById('errorMessage').innerText = 'Erro ao conectar ao servidor';
        $('#errorModal').modal('show');
    }
});