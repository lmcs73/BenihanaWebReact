import React, { useEffect } from "react";
import { Link } from 'react-router-dom';
import { Button } from 'reactstrap';

export const RunSimulation = props => {

    useEffect(() => {
        let id = props.match.params.id;
        console.log(id);
    })
  return (
    <div>
      <h4>Simulation</h4>
      <Link to="/">
        <Button color="primary">Close</Button>
      </Link>
    </div>
  );
};
