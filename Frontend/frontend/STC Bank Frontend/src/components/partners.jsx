import React from "react";
import currency from "../images/image-currency.jpg";
import plane from "../images/image-plane.jpg";
import restaurant from "../images/image-restaurant.jpg";
import confetti from "../images/image-confetti.jpg";

const Partners = () => {
  return (
    <section className="latest">
      <h3 className="latest-heading">Partners and Supscriptions</h3>
      <div className="cards-latest">
        <div className="card-block">
          <img src={currency} alt="currency" />
          <div className="text-block">
            <p className="author">By Wilson Hutton</p>
            <h4>Receive money in any currency with no fees</h4>
            <p>
              The world is getting smaller and we’re becoming more mobile. So
              why should you be forced to only receive money in a single …
            </p>
          </div>
        </div>
        <div className="card-block">
          <img src={restaurant} alt="restaurant" />
          <div className="text-block">
            <p className="author">By Wilson Hutton</p>

            <h4>Treat yourself without worrying about money</h4>
            <p>
              Our simple budgeting feature allows you to separate out your
              spending and set realistic limits each month. That means you …
            </p>
          </div>
        </div>
        <div className="card-block">
          <img src={plane} alt="plane" />
          <div className="text-block">
            <p className="author">By Wilson Hutton</p>

            <h4>Take your Easybank card wherever you go</h4>
            <p>
              We want you to enjoy your travels. This is why we don’t charge any
              fees on purchases while you’re abroad. We’ll even show you …
            </p>
          </div>
        </div>
        <div className="card-block">
          <img src={confetti} alt="confetti" />
          <div className="text-block">
            <p className="author">By Claire Robinson</p>
            <h4>Take your Easybank card wherever you go</h4>
            <p>
              Our invite-only Beta accounts are now live! After a lot of hard
              work by the whole team, we’re excited to launch our closed beta.
              It’s easy to request an invite through the site ...
            </p>
          </div>
        </div>
      </div>
    </section>
  );
};

export default Partners;
