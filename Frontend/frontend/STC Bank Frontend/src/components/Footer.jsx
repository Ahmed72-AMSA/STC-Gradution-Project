import React from "react";
import facebook from "../images/icon-facebook.svg";
import instagram from "../images/icon-instagram.svg";
import twitter from "../images/icon-twitter.svg";
import pinterest from "../images/icon-pinterest.svg";
import youtube from "../images/icon-youtube.svg";

const Footer = () => {
  return (
    <footer>
      <div className="logo-social">
        <div className="social-block">
          <img src={facebook} alt="facebook" />
          <img src={instagram} alt="instagram" />
          <img src={twitter} alt="twitter" />
          <img src={pinterest} alt="pinterest" />
          <img src={youtube} alt="youtube" />
        </div>
      </div>
      <div className="first">
        <a href="/">Home</a>
        <a href="/">About</a>
        <a href="/">Contact</a>
      </div>
      <div className="second">
        <a href="/">Careers</a>
        <a href="/">Support</a>
        <a href="/">Privacy Policy</a>
      </div>
      <div className="request-copy">
        <button className="btn-req" type="button">
          Have a question ?
        </button>
        <p>
          © STC. Developed by
          <a href="https://www.frontendmentor.io/profile/parvathyvd">
            {" "}
            STC Developers
          </a>
        </p>
      </div>
    </footer>
  );
};

export default Footer;
