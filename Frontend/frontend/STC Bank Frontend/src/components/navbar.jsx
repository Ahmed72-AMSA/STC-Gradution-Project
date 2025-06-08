import React from 'react';
import { Navbar, Nav, Button, Container, Dropdown } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import { FaBell, FaUserCircle, FaSignOutAlt } from 'react-icons/fa';
import logo from "../images/dashlogo.jpg";
import './dashboard.css';

const DashboardNavbar = () => {
    const navigate = useNavigate();

    const handleLogout = () => {
        navigate('/signup', { replace: true });
    };

    return (
        <Navbar bg="white" expand="lg" className="dashboard-navbar">
            <Container fluid>
                <Navbar.Brand href="#" className='dashboard-navbar-brand'>
                    <img 
                        src={logo} 
                        alt="STC Bank Logo"
                        className="dashboard-navbar-logo"
                    />
                    <span className="dashboard-navbar-brand-text">STC Bank</span>
                </Navbar.Brand>
                
                <Navbar.Toggle aria-controls="basic-navbar-nav" />
                
                <Navbar.Collapse id="basic-navbar-nav" className="justify-content-end">
                    <Nav className="align-items-center">
                        <Button variant="link" className="dashboard-notification-btn">
                            <FaBell size={20} />
                            <span className="dashboard-notification-badge">3</span>
                        </Button>
                    </Nav>
                </Navbar.Collapse>
            </Container>
        </Navbar>
    );
};

export default DashboardNavbar;