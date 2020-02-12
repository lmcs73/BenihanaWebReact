import React, { useState, useEffect } from "react";

export const Configuration = props => {
  const [batching, setBatching] = useState("0");
  const [layoutTable, setLayoutTable] = useState("15");
  const [diningTimeBeforePeak, setDiningTimeBeforePeak] = useState("60");
  const [diningTimeDuringPeak, setDiningTimeDuringPeak] = useState("60");
  const [diningTimeAfterPeak, setDiningTimeAfterPeak] = useState("60");
  const [advertisement, setAdvertisement] = useState("0");
  const [adsLevel, setAdsLevel] = useState("0");
  const [openingHour, setOpeningHour] = useState("17");

  const handleSubmit = () => {
    let data = {
      batching: parseInt(batching),
      layoutTable: parseInt(layoutTable),
      diningTimeBeforePeak: parseInt(diningTimeBeforePeak),
      diningTimeDuringPeak: parseInt(diningTimeDuringPeak),
      diningTimeAfterPeak: parseInt(diningTimeAfterPeak),
      advertisement: parseInt(advertisement),
      adsLevel: parseInt(adsLevel),
      openingHour: parseInt(openingHour)
    };
    props.onSubmitRun(data);
  };

  useEffect(() => {
    if (localStorage.getItem("config") !== null) {
      let config = JSON.parse(localStorage.getItem("config"));
      setBatching(config.batching.toString());
      setLayoutTable(config.layoutTable.toString());
      setDiningTimeBeforePeak(config.diningTimeBeforePeak.toString());
      setDiningTimeDuringPeak(config.diningTimeDuringPeak.toString());
      setDiningTimeAfterPeak(config.diningTimeAfterPeak.toString());
      setAdvertisement(config.advertisement.toString());
      setAdsLevel(config.adsLevel.toString());
      setOpeningHour(config.openingHour.toString());
    }
  }, []);

  return (
    <div className="row">
      <div className="col-sm">
        <h4>1. Batching Rule</h4>
        <form>
          <div className="form-check">
            <input
              className="form-check-input"
              type="radio"
              name="batchingRadios"
              id="batchingNone"
              value="0"
              checked={batching === "0"}
              onChange={changeEvent => setBatching(changeEvent.target.value)}
            />
            <label className="form-check-label" htmlFor="batchingNone">
              None
            </label>
          </div>
          <div className="form-check">
            <input
              className="form-check-input"
              type="radio"
              name="batchingRadios"
              id="batching8"
              value="1"
              checked={batching === "1"}
              onChange={changeEvent => setBatching(changeEvent.target.value)}
            />
            <label className="form-check-label" htmlFor="batching8">
              Table of 8
            </label>
          </div>
          <div className="form-check">
            <input
              className="form-check-input"
              type="radio"
              name="batchingRadios"
              id="batching4"
              value="2"
              checked={batching === "2"}
              onChange={changeEvent => setBatching(changeEvent.target.value)}
            />
            <label className="form-check-label" htmlFor="batching4">
              Table of 4
            </label>
          </div>
          <div className="form-check">
            <input
              className="form-check-input"
              type="radio"
              name="batchingRadios"
              id="batching4to8"
              value="3"
              checked={batching === "3"}
              onChange={changeEvent => setBatching(changeEvent.target.value)}
            />
            <label className="form-check-label" htmlFor="batching4to8">
              Table of 4-8
            </label>
          </div>
        </form>
      </div>
      <div className="col-sm">
        <h4>2. Layout Design</h4>
        <form>
          <div className="form-group">
            <label htmlFor="layoutTableControlRange">
              {layoutTable} dining tables.
            </label>
            <input
              type="range"
              min="11"
              max="19"
              value={layoutTable}
              onChange={changeEvent => setLayoutTable(changeEvent.target.value)}
              className="form-control-range"
              id="layoutTableControlRange"
            />
          </div>
        </form>
      </div>
      <div className="col-sm">
        <h4>3. Expected Dining Time</h4>
        <form>
          <div className="form-group">
            <label htmlFor="timingBeforePeakControlRange">
              Before Peak: {diningTimeBeforePeak} min.
            </label>
            <input
              type="range"
              min="45"
              max="75"
              value={diningTimeBeforePeak}
              onChange={changeEvent =>
                setDiningTimeBeforePeak(changeEvent.target.value)
              }
              className="form-control-range"
              id="timingBeforePeakControlRange"
            />
          </div>
          <div className="form-group">
            <label htmlFor="timingDuringPeakControlRange">
              During Peak: {diningTimeDuringPeak} min.
            </label>
            <input
              type="range"
              min="45"
              max="75"
              value={diningTimeDuringPeak}
              onChange={changeEvent =>
                setDiningTimeDuringPeak(changeEvent.target.value)
              }
              className="form-control-range"
              id="timingDuringPeakControlRange"
            />
          </div>
          <div className="form-group">
            <label htmlFor="timingAfterPeakControlRange">
              After Peak: {diningTimeAfterPeak} min.
            </label>
            <input
              type="range"
              min="45"
              max="75"
              value={diningTimeAfterPeak}
              onChange={changeEvent =>
                setDiningTimeAfterPeak(changeEvent.target.value)
              }
              className="form-control-range"
              id="timingAfterPeakControlRange"
            />
          </div>
        </form>
      </div>
      <div className="col-sm">
        <h4>4. Advertisement</h4>
        <form>
          <div className="form-check">
            <input
              className="form-check-input"
              type="radio"
              name="adRadios"
              id="awarenessBuilding"
              value="0"
              checked={advertisement === "0"}
              onChange={changeEvent =>
                setAdvertisement(changeEvent.target.value)
              }
            />
            <label className="form-check-label" htmlFor="awarenessBuilding">
              Awareness Building
            </label>
          </div>
          <div className="form-check">
            <input
              className="form-check-input"
              type="radio"
              name="adRadios"
              id="discountPromo"
              value="1"
              checked={advertisement === "1"}
              onChange={changeEvent =>
                setAdvertisement(changeEvent.target.value)
              }
            />
            <label className="form-check-label" htmlFor="discountPromo">
              Discount Promotion
            </label>
          </div>
          <div className="form-check">
            <input
              className="form-check-input"
              type="radio"
              name="adRadios"
              id="happyHour"
              value="2"
              checked={advertisement === "2"}
              onChange={changeEvent =>
                setAdvertisement(changeEvent.target.value)
              }
            />
            <label className="form-check-label" htmlFor="happyHour">
              Happy Hour
            </label>
          </div>
          <div className="form-group">
            <label htmlFor="adsLevelControlRange">Ads Level: {adsLevel}</label>
            <input
              type="range"
              min="0"
              max="4"
              value={adsLevel}
              onChange={changeEvent => setAdsLevel(changeEvent.target.value)}
              className="form-control-range"
              id="adsLevelControlRange"
            />
          </div>
        </form>
      </div>
      <div className="col-sm">
        <h4>Opening Hour</h4>
        <form>
          <div className="form-check">
            <input
              className="form-check-input"
              type="radio"
              name="openingRadios"
              id="17"
              value="17"
              checked={openingHour === "17"}
              onChange={changeEvent => setOpeningHour(changeEvent.target.value)}
            />
            <label className="form-check-label" htmlFor="17">
              17:00
            </label>
          </div>
          <div className="form-check">
            <input
              className="form-check-input"
              type="radio"
              name="openingRadios"
              id="18"
              value="18"
              checked={openingHour === "18"}
              onChange={changeEvent => setOpeningHour(changeEvent.target.value)}
            />
            <label className="form-check-label" htmlFor="18">
              18:00
            </label>
          </div>
          <div className="form-check">
            <input
              className="form-check-input"
              type="radio"
              name="openingRadios"
              id="19"
              value="19"
              checked={openingHour === "19"}
              onChange={changeEvent => setOpeningHour(changeEvent.target.value)}
            />
            <label className="form-check-label" htmlFor="19">
              19:00
            </label>
          </div>
        </form>
      </div>
      <div className="col-sm">
        <button className="btn btn-primary" onClick={handleSubmit}>
          RUN
        </button>
      </div>
    </div>
  );
};
