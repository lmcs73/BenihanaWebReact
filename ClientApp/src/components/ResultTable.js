import React from "react";
import { withRouter } from "react-router-dom";
import axios from "axios";

const ResultTable = props => {
  let averageBar = 0;
  let averageDinner = 0;
  let averageRevenue = 0;
  let averageCost = 0;
  let averageProfit = 0;

  if (props.results.length > 0) {
    let resultLength = props.results.length;
    let barSum = 0;
    let dinnerSum = 0;
    let revenueSum = 0;
    let costSum = 0;
    let profitSum = 0;
    props.results.forEach(result => {
      barSum += result.barDrinksSold;
      dinnerSum += result.diningRoomDinnersServed;
      revenueSum += result.financialRevenueTotal;
      costSum += result.financialCostTotal;
      profitSum += result.financialNightProfit;
    });
    averageBar = barSum / resultLength;
    averageDinner = dinnerSum / resultLength;
    averageRevenue = revenueSum / resultLength;
    averageCost = costSum / resultLength;
    averageProfit = profitSum / resultLength;
  }

  const handleRun = async index => {
    let config = JSON.parse(localStorage.getItem("config"));
    let connectionId = localStorage.getItem("signalRConnectionId");
    let payload = { config, connectionId };
    await axios.post(`api/simulation/run/${index}`, payload, {
      headers: { "Content-Type": "application/json" }
    });
    props.history.push(`/run/${index}`);
  };

  return (
    <table className="table">
      <thead>
        <tr>
          <th scope="col">#</th>
          <th scope="col">Drinks Sold</th>
          <th scope="col">Dinners Served</th>
          <th scope="col">Total Revenue</th>
          <th scope="col">Total Cost</th>
          <th scope="col">Night Profit</th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        {props.results.map((result, index) => {
          return (
            <tr key={index}>
              <th scope="row">{index}</th>
              <td>{result.barDrinksSold}</td>
              <td>{result.diningRoomDinnersServed}</td>
              <td>${result.financialRevenueTotal}</td>
              <td>${result.financialCostTotal.toFixed(2)}</td>
              <td>${result.financialNightProfit.toFixed(2)}</td>
              <td>
                <button
                  className="btn btn-outline-info"
                  onClick={() => handleRun(index)}
                >
                  PLAY
                </button>
              </td>
            </tr>
          );
        })}
        {props.results.length > 0 && (
          <tr key="average">
            <th scope="row">Average</th>
            <td>{averageBar}</td>
            <td>{averageDinner}</td>
            <td>${averageRevenue}</td>
            <td>${averageCost.toFixed(2)}</td>
            <td>${averageProfit.toFixed(2)}</td>
          </tr>
        )}
      </tbody>
    </table>
  );
};

export default withRouter(ResultTable);
