import React, { useState, useEffect } from 'react';
import { 
    Table, 
    Button, 
    Modal, 
    Form, 
    Spinner,
    Badge,
    InputGroup,
    FormControl,
    Card,
    Dropdown
} from 'react-bootstrap';
import { ToastContainer, toast } from 'react-toastify';
import { 
    FaSearch, 
    FaPlus, 
    FaFilter, 
    FaDownload, 
    FaEllipsisV,
    FaEdit,
    FaTrash,
    FaUserShield,
    FaUserCheck,
    FaIdCard
} from 'react-icons/fa';
import './dashboard.css';

const DashboardUsers = () => {
    const [loading, setLoading] = useState(true);
    const [users, setUsers] = useState([]);
    const [showModal, setShowModal] = useState(false);
    const [searchTerm, setSearchTerm] = useState('');
    const [nationalIdSearch, setNationalIdSearch] = useState('');
    const [currentUser, setCurrentUser] = useState({
        id: '',
        userName: '',
        email: '',
        role: '',
        phoneNumber: '',
        nationalID: '',
        isSuspended: false,
        userCreatedAt: ''
    });

    useEffect(() => {
        fetchAllUsers();
    }, []);

    const fetchAllUsers = async () => {
        setLoading(true);
        try {
            const response = await fetch('https://localhost:7152/api/Users');
            if (!response.ok) {
                throw new Error('Failed to fetch users');
            }
            const data = await response.json();
            setUsers(data);
            setLoading(false);
        } catch (error) {
            toast.error('Failed to fetch users: ' + error.message);
            setLoading(false);
        }
    };

    const handleSearchByNationalId = async () => {
        if (!nationalIdSearch.trim()) {
            toast.warning('Please enter a National ID to search');
            return;
        }

        setLoading(true);
        try {
            const response = await fetch(`https://localhost:7152/api/Users/by-nationalid/${nationalIdSearch}`);
            if (!response.ok) {
                throw new Error('User not found');
            }
            const user = await response.json();
            setUsers([user]);
            setLoading(false);
        } catch (error) {
            toast.error('User not found: ' + error.message);
            setLoading(false);
        }
    };

    const handleResetSearch = () => {
        setNationalIdSearch('');
        fetchAllUsers();
    };

    const handleEdit = (user) => {
        setCurrentUser(user);
        setShowModal(true);
    };

    const handleSave = async () => {
        try {
            const response = await fetch(`https://localhost:7152/api/Users/${currentUser.id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(currentUser)
            });

            if (!response.ok) {
                throw new Error('Failed to update user');
            }

            setUsers(users.map(user => 
                user.id === currentUser.id ? currentUser : user
            ));
            
            toast.success('User updated successfully');
            setShowModal(false);
        } catch (error) {
            toast.error('Failed to update user: ' + error.message);
        }
    };

    const handleDelete = async (id) => {
        if (window.confirm('Are you sure you want to delete this user?')) {
            try {
                const response = await fetch(`https://localhost:7152/api/Users/${id}`, {
                    method: 'DELETE'
                });

                if (!response.ok) {
                    throw new Error('Failed to delete user');
                }

                setUsers(users.filter(user => user.id !== id));
                toast.success('User deleted successfully');
            } catch (error) {
                toast.error('Failed to delete user: ' + error.message);
            }
        }
    };

    const handleSuspend = async (id) => {
        try {
            const response = await fetch(`https://localhost:7152/api/Login/SuspendUser/${id}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ isSuspended: true })
            });

            if (!response.ok) {
                throw new Error('Failed to suspend user');
            }

            setUsers(users.map(user => 
                user.id === id ? { ...user, isSuspended: true } : user
            ));
            
            toast.success('User suspended successfully');
        } catch (error) {
            toast.error('Failed to suspend user: ' + error.message);
        }
    };

    const handleUnsuspend = async (id) => {
        try {
            const response = await fetch(`https://localhost:7152/api/Login/UnsuspendUser/${id}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ isSuspended: false })
            });

            if (!response.ok) {
                throw new Error('Failed to unsuspend user');
            }

            setUsers(users.map(user => 
                user.id === id ? { ...user, isSuspended: false } : user
            ));
            
            toast.success('User unsuspended successfully');
        } catch (error) {
            toast.error('Failed to unsuspend user: ' + error.message);
        }
    };

    const filteredUsers = users.filter(user =>
        user.userName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        user.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
        user.phoneNumber.toLowerCase().includes(searchTerm.toLowerCase()) ||
        user.nationalID.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const getStatusVariant = (isSuspended) => {
        return isSuspended ? 'danger' : 'success';
    };

    const getStatusText = (isSuspended) => {
        return isSuspended ? 'Suspended' : 'Active';
    };

    const getRoleVariant = (role) => {
        switch (role) {
            case 'Admin': return 'primary';
            case 'Manager': return 'info';
            default: return 'danger';
        }
    };

    const formatDate = (dateString) => {
        const options = { year: 'numeric', month: 'short', day: 'numeric' };
        return new Date(dateString).toLocaleDateString(undefined, options);
    };

    return (
        <div className="dashboard-content-wrapper">
            <ToastContainer position="top-right" autoClose={3000} />
            
            <div className="dashboard-content-header">
                <div className="dashboard-header-title">
                    <h2>User Management</h2>
                    <p className="dashboard-subtitle">Manage all system users and their permissions</p>
                </div>
                
                <div className="dashboard-header-actions">
                    <InputGroup className="dashboard-search-bar">
                        <InputGroup.Text>
                            <FaSearch />
                        </InputGroup.Text>
                        <FormControl 
                            placeholder="Search users by name, email, phone or national ID..." 
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                        />
                    </InputGroup>
                    
                    <InputGroup className="dashboard-nationalid-search">
                        <InputGroup.Text>
                            <FaIdCard />
                        </InputGroup.Text>
                        <FormControl 
                            placeholder="Search by National ID..." 
                            value={nationalIdSearch}
                            onChange={(e) => setNationalIdSearch(e.target.value)}
                            className='p-3'
                        />
                        <Button 
                            variant="outline-success" 
                            onClick={handleSearchByNationalId}
                        >
                            <FaSearch className="me-2" /> Search
                        </Button>
                        {nationalIdSearch && (
                            <Button 
                                variant="outline-danger" 
                                onClick={handleResetSearch}
                            >
                                Reset
                            </Button>
                        )}
                    </InputGroup>
                </div>
            </div>

            <div className="dashboard-stats-cards">
                <Card className="dashboard-stat-card">
                    <Card.Body>
                        <h6>Total Users</h6>
                        <h3>{users.length}</h3>
                    </Card.Body>
                </Card>
                
                <Card className="dashboard-stat-card">
                    <Card.Body>
                        <h6>Active Users</h6>
                        <h3>{users.filter(u => !u.isSuspended).length}</h3>
                    </Card.Body>
                </Card>
                
                <Card className="dashboard-stat-card">
                    <Card.Body>
                        <h6>Admin Users</h6>
                        <h3>{users.filter(u => u.role === 'Admin').length}</h3>
                    </Card.Body>
                </Card>
                
                <Card className="dashboard-stat-card">
                    <Card.Body>
                        <h6>Suspended Users</h6>
                        <h3>{users.filter(u => u.isSuspended).length}</h3>
                    </Card.Body>
                </Card>
            </div>

            <Card className="dashboard-main-card">
                <Card.Body>
                    {loading ? (
                        <div className="dashboard-loading-spinner">
                            <Spinner animation="border" variant="success" />
                        </div>
                    ) : (
                        <div className="dashboard-table-responsive">
                            <Table hover className="dashboard-users-table">
                                <thead>
                                    <tr>
                                        <th>ID</th>
                                        <th>User</th>
                                        <th>Contact</th>
                                        <th>Role</th>
                                        <th>Status</th>
                                        <th>Joined Date</th>
                                        <th>Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {filteredUsers.map(user => (
                                        <tr key={user.id}>
                                            <td>#{user.id}</td>
                                            <td>
                                                <div className="dashboard-user-info">
                                                    <div className="dashboard-user-avatar">
                                                        {user.userName.charAt(0)}
                                                    </div>
                                                    <div className="dashboard-user-details">
                                                        <strong>{user.userName}</strong>
                                                        <small>{user.nationalID}</small>
                                                    </div>
                                                </div>
                                            </td>
                                            <td>
                                                <div className="dashboard-user-details">
                                                    <small>{user.email}</small>
                                                    <small>{user.phoneNumber}</small>
                                                </div>
                                            </td>
                                            <td>
                                                <Badge bg={getRoleVariant(user.role)}>
                                                    {user.role}
                                                </Badge>
                                            </td>
                                            <td>
                                                <Badge pill bg={getStatusVariant(user.isSuspended)}>
                                                    {getStatusText(user.isSuspended)}
                                                </Badge>
                                            </td>
                                            <td>
                                                {formatDate(user.userCreatedAt)}
                                            </td>
                                            <td>
                                                <Dropdown>
                                                    <Dropdown.Toggle variant="link" className="dashboard-action-dropdown">
                                                        <FaEllipsisV />
                                                    </Dropdown.Toggle>
                                                    <Dropdown.Menu>
                                                        <Dropdown.Item onClick={() => handleEdit(user)}>
                                                            <FaEdit className="me-2" /> Edit
                                                        </Dropdown.Item>
                                                        {user.isSuspended ? (
                                                            <Dropdown.Item onClick={() => handleUnsuspend(user.id)}>
                                                                <FaUserCheck className="me-2" />
                                                                Unsuspend
                                                            </Dropdown.Item>
                                                        ) : (
                                                            <Dropdown.Item onClick={() => handleSuspend(user.id)}>
                                                                <FaUserShield className="me-2" />
                                                                Suspend
                                                            </Dropdown.Item>
                                                        )}
                                                        <Dropdown.Divider />
                                                        <Dropdown.Item 
                                                            onClick={() => handleDelete(user.id)}
                                                            className="text-danger"
                                                        >
                                                            <FaTrash className="me-2" /> Delete
                                                        </Dropdown.Item>
                                                    </Dropdown.Menu>
                                                </Dropdown>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </Table>
                        </div>
                    )}
                </Card.Body>
            </Card>

            <Modal show={showModal} onHide={() => setShowModal(false)} centered size="lg">
                <Modal.Header closeButton className="dashboard-modal-header">
                    <Modal.Title>Edit User</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <div className="dashboard-user-modal-header">
                        <div className="dashboard-user-avatar-lg">
                            {currentUser.userName.charAt(0)}
                        </div>
                        <div className="dashboard-user-info-lg">
                            <h5>{currentUser.userName}</h5>
                            <p>{currentUser.email}</p>
                            <Badge bg="light" text="dark">
                                Member since {formatDate(currentUser.userCreatedAt)}
                            </Badge>
                        </div>
                    </div>
                    
                    <Form>
                        <div className="row">
                            <div className="col-md-6">
                                <Form.Group className="mb-3">
                                    <Form.Label>Full Name</Form.Label>
                                    <Form.Control 
                                        type="text" 
                                        value={currentUser.userName} 
                                        onChange={(e) => setCurrentUser({...currentUser, userName: e.target.value})}
                                    />
                                </Form.Group>
                            </div>
                            <div className="col-md-6">
                                <Form.Group className="mb-3">
                                    <Form.Label>Email Address</Form.Label>
                                    <Form.Control 
                                        type="email" 
                                        value={currentUser.email} 
                                        onChange={(e) => setCurrentUser({...currentUser, email: e.target.value})}
                                    />
                                </Form.Group>
                            </div>
                        </div>
                        
                        <div className="row">
                            <div className="col-md-6">
                                <Form.Group className="mb-3">
                                    <Form.Label>Phone Number</Form.Label>
                                    <Form.Control 
                                        type="tel" 
                                        value={currentUser.phoneNumber} 
                                        onChange={(e) => setCurrentUser({...currentUser, phoneNumber: e.target.value})}
                                    />
                                </Form.Group>
                            </div>
                            <div className="col-md-6">
                                <Form.Group className="mb-3">
                                    <Form.Label>National ID</Form.Label>
                                    <Form.Control 
                                        type="text" 
                                        value={currentUser.nationalID} 
                                        onChange={(e) => setCurrentUser({...currentUser, nationalID: e.target.value})}
                                    />
                                </Form.Group>
                            </div>
                        </div>
                        
                        <div className="row">
                            <div className="col-md-6">
                                <Form.Group className="mb-3">
                                    <Form.Label>Role</Form.Label>
                                    <Form.Select
                                        value={currentUser.role}
                                        onChange={(e) => setCurrentUser({...currentUser, role: e.target.value})}
                                    >
                                        <option value="User">User</option>
                                        <option value="Admin">Admin</option>
                                        <option value="Manager">Manager</option>
                                    </Form.Select>
                                </Form.Group>
                            </div>
                            <div className="col-md-6">
                                <Form.Group className="mb-3">
                                    <Form.Label>Account Status</Form.Label>
                                    <Form.Select
                                        value={currentUser.isSuspended ? 'Suspended' : 'Active'}
                                        onChange={(e) => setCurrentUser({
                                            ...currentUser, 
                                            isSuspended: e.target.value === 'Suspended'
                                        })}
                                    >
                                        <option value="Active">Active</option>
                                        <option value="Suspended">Suspended</option>
                                    </Form.Select>
                                </Form.Group>
                            </div>
                        </div>
                    </Form>
                </Modal.Body>
                <Modal.Footer className="dashboard-modal-footer">
                    <Button variant="light" onClick={() => setShowModal(false)}>
                        Cancel
                    </Button>
                    <Button variant="primary" onClick={handleSave}>
                        Save Changes
                    </Button>
                </Modal.Footer>
            </Modal>
        </div>
    );
};

export default DashboardUsers;