import React, { useState } from "react";
import logo from "../images/logo.png";
import hamburger from "../images/icon-hamburger.svg";
import close from "../images/icon-close.svg";

const Header = () => {
  const [showMenu, setShowMenu] = useState(false);
  return (
    <header>
<div className="logo pl-8" style={{ display: 'flex', alignItems: 'center' }}>
  <img src={logo} alt="Logo" style={{ width: '50px', marginRight: '10px' }} />
  <h2 style={{fontSize:"3rem",marginTop:"4px"}}>STC</h2>
</div>

      <nav>
        <a href="/">Home</a>
        <a href="/">About</a>
        <a href="/">Contact</a>
        <a href="/">Blog</a>
        <a href="/">Careers</a>
      </nav>
      <div className="request-btn pr-8">
        <button className="btn-req">Logout</button>
      </div>
      <div className="mobile-icon">
        {!showMenu && (
          <img
            src={hamburger}
            alt="icon hamgurger"
            onClick={() => setShowMenu(!showMenu)}
          />
        )}
        {showMenu && (
          <img
            src={close}
            alt="icon close"
            onClick={() => setShowMenu(!showMenu)}
          />
        )}
      </div>
      {showMenu && (
        <div className="overlay">
          <div className={showMenu ? "mobile-nav" : "hide"}>
            <a href="/">Home</a>
            <a href="/">About</a>
            <a href="/">Contact</a>
            <a href="/">Blog</a>
            <a href="/">Careers</a>
          </div>
        </div>
      )}
    </header>
  );
};

export default Header;
