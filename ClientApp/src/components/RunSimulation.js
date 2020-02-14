import React, { useState, useEffect } from "react";
import { Button } from "reactstrap";
import {
  LineChart,
  AreaChart,
  ComposedChart,
  Line,
  Area,
  YAxis,
  Tooltip,
  Legend,
  ResponsiveContainer
} from "recharts";

export const RunSimulation = props => {
  const [data, setData] = useState([]);
  useEffect(() => {
    // let id = props.match.params.id;
    // console.log(id);
    // props.hubConnection.on("setSimulationResults", results => {
    //   console.log(`Received simulation results ${results}`);
    // });
    console.log(props.result);
    let consolidatedData = [];
    for (let i = 0; i < props.result.times.length; i++) {
      consolidatedData.push({
        customersInBar: props.result.customersInBar[i],
        customersInDiningRoom: props.result.customersInDinningRoom[i],
        customersLost: props.result.customersLost[i],
        totalServedDrinks: props.result.totalServedDrinks[i],
        totalServedDinners: props.result.totalServedDinners[i]
      });
    }
    setData(consolidatedData);
  }, []);

  const renderCustLostChart = (
    <ResponsiveContainer width="95%" height={750}>
      <ComposedChart data={data}>
        <YAxis />
        <Tooltip />
        <Legend />
        <Line
          type="monotone"
          dataKey="customersInBar"
          name="Customers In Bar"
          stroke="#003f5c"
          dot={false}
        />
        <Line
          type="monotone"
          dataKey="customersInDiningRoom"
          name="Customers In Dining Room"
          stroke="#58508d"
          dot={false}
        />
        <Area
          type="monotone"
          dataKey="customersLost"
          name="Customers Lost"
          fill="#e06e7c"
          fillOpacity={0.6}
          stroke="#ba5079"
          dot={false}
        />
        <Line
          type="monotone"
          dataKey="totalServedDrinks"
          name="Total Drinks"
          stroke="#ff6361"
          dot={false}
        />
        <Line
          type="monotone"
          dataKey="totalServedDinners"
          name="Total Dinner"
          stroke="#ffa600"
          dot={false}
        />
      </ComposedChart>
    </ResponsiveContainer>
  );

  return (
    <div className="container">
      <h4>Simulation</h4>
      {renderCustLostChart}
      <div className="row justify-content-center">
        <Button onClick={() => props.onClose()} color="primary">
          Close
        </Button>
      </div>
    </div>
  );
};
