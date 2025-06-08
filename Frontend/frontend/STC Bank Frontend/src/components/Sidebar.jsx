import React from 'react';
import {
    FaUserAlt,
    FaChartLine,
    FaCreditCard,
    FaExchangeAlt,
    FaCog,
    FaQuestionCircle,
    FaPiggyBank,
    FaHandHoldingUsd,
    FaShieldAlt
} from "react-icons/fa";
import { NavLink } from 'react-router-dom';
import './dashboard.css';
import { FaPerson } from 'react-icons/fa6';

const DashboardSidebar = ({ children }) => {
    const menuItem = [
        {
            path: "/dashboard/users",
            name: "Users Managment",
            icon: <FaPerson />,
            badge: ""
        },
        {
            path: "/dashboard/accounts",
            name: "Accounts",
            icon: <FaUserAlt />,
            badge: "3"
        },
        {
            path: "/dashboard/transactions",
            name: "Transactions",
            icon: <FaExchangeAlt />,
            badge: "New"
        },
        {
            path: "/dashboard/cards",
            name: "Cards",
            icon: <FaCreditCard />,
            badge: ""
        },
        {
            path: "/dashboard/loans",
            name: "Loans",
            icon: <FaHandHoldingUsd />,
            badge: ""
        },
        {
            path: "/dashboard/savings",
            name: "Savings",
            icon: <FaPiggyBank />,
            badge: ""
        },
        {
            path: "/dashboard/security",
            name: "Security",
            icon: <FaShieldAlt />,
            badge: ""
        },
        {
            path: "/dashboard/settings",
            name: "Settings",
            icon: <FaCog />,
            badge: ""
        },
        {
            path: "/dashboard/support",
            name: "Support",
            icon: <FaQuestionCircle />,
            badge: "24/7"
        }
    ];

    return (
        <div className="dashboard-container">
            <div className="dashboard-sidebar-wrapper">
                <div className="dashboard-sidebar">
                    <div className="dashboard-sidebar-header">
                        <div className="dashboard-bank-info">
                            <h1 className="dashboard-sidebar-title">
                                <span className="dashboard-text-primary">STC</span>
                                <span className="dashboard-text-success"> Bank</span>
                            </h1>
                            <div className="dashboard-bank-status">
                                <span className="dashboard-status-dot"></span>
                                <span>Online Banking</span>
                            </div>
                        </div>
                    </div>
                    <div className="dashboard-sidebar-menu">
                        <div className="dashboard-sidebar-divider">
                            <span>MAIN NAVIGATION</span>
                        </div>
                        {menuItem.map((item, index) => (
                            <NavLink 
                                to={item.path} 
                                key={index} 
                                className={({ isActive }) => 
                                    isActive ? "dashboard-sidebar-link dashboard-sidebar-link-active" : "dashboard-sidebar-link"
                                }
                            >
                                <div className="dashboard-sidebar-icon">{item.icon}</div>
                                <div className="dashboard-sidebar-text">{item.name}</div>
                                {item.badge && (
                                    <div className="dashboard-sidebar-badge">{item.badge}</div>
                                )}
                            </NavLink>
                        ))}
                    </div>
                
                </div>
                <main className="dashboard-main-content">{children}</main>
            </div>
        </div>
    );
};

export default DashboardSidebar;