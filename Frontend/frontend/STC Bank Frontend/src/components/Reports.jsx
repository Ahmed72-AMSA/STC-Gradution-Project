import React from "react";
import currency from "../images/image-currency.jpg";
import plane from "../images/image-plane.jpg";
import restaurant from "../images/image-restaurant.jpg";
import confetti from "../images/image-confetti.jpg";

const Reports = () => {
  return (
    <section className="home-reports">
      <h3 className="latest-heading">Get Your Financial Reciet Now!</h3>
      <div className="cards-latest">
        {/* Card 1 - Transaction Report */}
        <div className="card-block">
          <img src={currency} alt="currency" />
          <div className="text-block">
            <h4 className="mt-3">Transaction Reciet</h4>
            <p>
              View detailed reports of all your transactions.
            </p>
           
          </div>
        </div>

        {/* Card 2 - Subscription Report */}
        <div className="card-block">
          <img src={currency} alt="restaurant" />
          <div className="text-block">
            <h4 className="mt-3">Subscription Report</h4>
            <p>
              Track all your active and past subscriptions.
            </p>
           
          </div>
        </div>

        {/* Card 3 - Zakah Report */}
        <div className="card-block">
          <img src={currency} alt="plane" />
          <div className="text-block">
            <h4 className="mt-3">Zakah Reciet</h4>
            <p>
              Calculate your eligible zakah amount.
            </p>
           
          </div>
        </div>

        {/* Card 4 - Loan Report */}
        <div className="card-block">
          <img src={currency} alt="confetti" />
          <div className="text-block">
            <h4 className="mt-3">Loan Reciet</h4>
            <p>
              View your loan repayment schedule.
            </p>
            <button className="card-action-btn">
              View Loan Details
            </button>
          </div>
        </div>
      </div>
    </section>
  );
};

export default Reports;