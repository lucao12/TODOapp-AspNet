function togglePassword() {
    const passwordField = document.getElementById('password');
    const toggleButton = document.getElementById('btn-password');
    if (passwordField.type === 'password') {
        passwordField.type = 'text';
        toggleButton.textContent = '🙈';
    } else {
        passwordField.type = 'password';
        toggleButton.textContent = '👁️';
    }
}

function toggleConfirmPassword() {
    const confirmPasswordField = document.getElementById('confirmPassword');
    const toggleButton = document.getElementById('btn-confirm-password');
    if (confirmPasswordField.type === 'password') {
        confirmPasswordField.type = 'text';
        toggleButton.textContent = '🙈';
    } else {
        confirmPasswordField.type = 'password';
        toggleButton.textContent = '👁️';
    }
}

function login(){
    window.location.href = 'index.html';
}
window.login = login;

document.getElementById('registerForm').addEventListener('submit', async function (event) {
    event.preventDefault();

    const name = document.getElementById('name').value;
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;
    const confirmPassword = document.getElementById('confirmPassword').value;

    try {
        const response = await fetch('http://192.168.0.99:5203/signup', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ name, email, password, confirmPassword }),
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
        }
        catch{
            document.getElementById('errorMessage').innerText = 'Erro ao logar';
        $('#errorModal').modal('show');
        }
    } catch (error) {
        document.getElementById('errorMessage').innerText = 'Erro ao conectar ao servidor';
        $('#errorModal').modal('show');
    }
});
