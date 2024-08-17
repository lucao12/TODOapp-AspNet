document.addEventListener('DOMContentLoaded', function () {
    const token = localStorage.getItem('token');

    if (!token) {
        alert('Usuário não autenticado. Redirecionando para a página de login.');
        window.location.href = 'index.html';
        return;
    }

    let req = "";
    const urlParams = new URLSearchParams(window.location.search);
    const param = urlParams.get('param');

    if (param === '1') {
        req = "Users";
        document.getElementById('userSection').style.display = 'block';
        console.log('Usuário é Admin');
    } else if (param === '2') {
        req = "Tasks";
        document.getElementById('taskSection').style.display = 'block';
        console.log('Usuário não é Admin');
    } else {
        alert('Usuário não autenticado. Redirecionando para a página de login.');
        localStorage.removeItem('token');
        window.location.href = 'index.html';
        console.log('Parâmetro inesperado ou ausente');
    }

    function logout() {
        localStorage.removeItem('token');
        window.location.href = 'index.html';
    }
    window.logout = logout;

    const apiUrl = "http://192.168.0.99:5203";

    async function editItem(id) {
        try {
            let requisicao = req === 'Users' ? 'user' : 'task';
            const response = await fetch(`${apiUrl}/${requisicao}/${id}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
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

            const item = await response.json();
            if (req === 'Users') {
                document.getElementById('editUserId').value = item.id;
                document.getElementById('editName').value = item.name;
                document.getElementById('editEmail').value = item.email;
                $('#editUserModal').modal('show');
            } else {
                document.getElementById('editTaskId').value = item.id;
                document.getElementById('editTitle').value = item.title;
                document.getElementById('editDescription').value = item.description;
                const dateLimit = new Date(item.dateLimit);
                dateLimit.setHours(dateLimit.getHours() - 3);

                document.getElementById('editDateLimit').value = dateLimit.toISOString().slice(0, 16);
                document.getElementById('editPriority').value = item.priority;
                document.getElementById('editIsCompleted').checked = item.isCompleted;
                $('#editTaskModal').modal('show');
            }
        } catch (error) {
            console.error(error);
        }
    }
    window.editItem = editItem;

    async function deleteItem(id) {
        try {
            let requisicao = req === 'Users' ? 'user' : 'task';
            const response = await fetch(`${apiUrl}/delete/${requisicao}/${id}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
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
            alert('Item deletado com sucesso');
            location.reload();
        } catch (error) {
            console.error(error);
        }
    }
    window.deleteItem = deleteItem;

    async function loadItems() {
        try {
            let requisicao = req === 'Users' ? 'users' : 'tasks';
            const response = await fetch(`${apiUrl}/${requisicao}`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
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

            const items = await response.json();
            const listElement = req === 'Users' ? document.getElementById('userList') : document.getElementById('taskList');
            listElement.innerHTML = '';
            items.forEach(item => {
                const itemElement = document.createElement('div');
                itemElement.classList.add('card', 'mb-2');
                itemElement.innerHTML = `
                    <div class="card-body">
                        ${req === 'Users' ? `
                            <h5 class="card-title">${item.name}</h5>
                            <p class="card-text">Email: ${item.email}</p>
                            <p class="card-text">Role: ${item.role}</p>
                            <button class="edit-btn" onclick="editItem(${item.id})">
                            <img src="../img/pencil-writing-on-a-paper-sheet_icon-icons.com_70422.ico" alt="Edit" style="width: 20px; height: 20px;">
                        </button>
                        ` : `
                            <h5 class="card-title">${item.title}</h5>
                            <p class="card-text">Descrição: ${item.description}</p>
                            <p class="card-text">Dia Limite: ${new Date(item.dateLimit).toLocaleString()}</p>
                            <p class="card-text">Prioridade: ${item.priority}</p>
                            <p class="card-text">Completo: ${item.isCompleted ? 'Sim' : 'Não'}</p>
                            <button class="edit-btn"  onclick="editItem(${item.id})">
                            <img src="../img/pencil-writing-on-a-paper-sheet_icon-icons.com_70422.ico" alt="Edit" style="width: 20px; height: 20px;">
                        </button>
                        `}
                        <button class="delete-btn" onclick="deleteItem(${item.id})">
                            <img src="../img/deletetrashbin_87227.ico" alt="Delete" style="width: 20px; height: 20px;">
                        </button>
                    </div>
                `;
                if (req === 'Tasks' && item.isCompleted) {
                    itemElement.classList.add('completed-task');
                    itemElement.innerHTML += `
                        <svg class="completed-icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                            <path d="M9 16.2l-3.5-3.5 1.4-1.4 2.1 2.1 5.7-5.7 1.4 1.4L9 16.2z"/>
                        </svg>`;
                }
                listElement.appendChild(itemElement);
            });
        } catch (error) {
            console.error(error);
        }
    }
    loadItems();

    document.getElementById('addTaskForm').addEventListener('submit', async function (e) {
        e.preventDefault();
        try {
            const task = {
                title: document.getElementById('title').value,
                description: document.getElementById('description').value,
                dateLimit: document.getElementById('dateLimit').value,
                priority: document.getElementById('priority').value,
                isCompleted: document.getElementById('isCompleted').checked
            };

            const response = await fetch(`${apiUrl}/add/task`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(task)
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

            alert('Tarefa criada com sucesso');
            $('#addTaskModal').modal('hide');
            location.reload();
        } catch (error) {
            console.error(error);
        }
    });

    document.getElementById('addUserForm').addEventListener('submit', async function (e) {
        e.preventDefault();
        try {
            const user = {
                name: document.getElementById('userName').value,
                email: document.getElementById('userEmail').value,
                password: document.getElementById('userPassword').value,
                confirmpassword: document.getElementById('userConfirmPassword').value,
            };

            const response = await fetch(`${apiUrl}/signup`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(user)
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

            alert('Usuário criado com sucesso');
            $('#addUserModal').modal('hide');
            location.reload();
        } catch (error) {
            console.error(error);
        }
    });

    document.getElementById('editTaskForm').addEventListener('submit', async function (e) {
        e.preventDefault();
        try {
            const id = document.getElementById('editTaskId').value;
            const task = {
                title: document.getElementById('editTitle').value,
                description: document.getElementById('editDescription').value,
                dateLimit: document.getElementById('editDateLimit').value,
                priority: document.getElementById('editPriority').value,
                isCompleted: document.getElementById('editIsCompleted').checked
            };

            const response = await fetch(`${apiUrl}/update/task/${id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(task)
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

            alert('Tarefa editada com sucesso');
            $('#editTaskModal').modal('hide');
            location.reload();
        } catch (error) {
            console.error(error);
        }
    });

    document.getElementById('editUserForm').addEventListener('submit', async function (e) {
        e.preventDefault();
        try {
            const id = document.getElementById('editUserId').value;
            const user = {
                name: document.getElementById('editName').value,
                email: document.getElementById('editEmail').value,
                password: document.getElementById('editPassword').value != "" ?  document.getElementById('editPassword').value : null,
            };

            const response = await fetch(`${apiUrl}/update/user/${id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(user)
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

            alert('Usuário editado com sucesso');
            $('#editUserModal').modal('hide');
            location.reload();
        } catch (error) {
            console.error(error);
        }
    });

    async function openEditUserModal() {
        try {
            const response = await fetch(`${apiUrl}/user/yourself`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
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

            const item = await response.json();
            document.getElementById('editUserYId').value = item.id;
            document.getElementById('editNameY').value = item.name;
            document.getElementById('editEmailY').value = item.email;
            $('#editUserModalYourself').modal('show');
            
        } catch (error) {
            console.error(error);
        }
        
    }
    window.openEditUserModal = openEditUserModal;

    document.getElementById('editUserYForm').addEventListener('submit', async function (e) {
        e.preventDefault();
        try {
            const user = {
                name: document.getElementById('editNameY').value,
                email: document.getElementById('editEmailY').value,
                oldpassword: document.getElementById('editPasswordY').value,
                newpassword: document.getElementById('editNewPasswordY').value != "" ? document.getElementById('editNewPasswordY').value : null
            };

            const response = await fetch(`${apiUrl}/update/user/yourself`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(user)
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

            alert('Usuário editado com sucesso');
            $('#editUserModalYourself').modal('hide');
            location.reload();
        } catch (error) {
            console.error(error);
        }
    });
});
