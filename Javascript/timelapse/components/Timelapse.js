import { useState } from 'react'
import style from './Timelapse.module.css'
import axios from 'axios'
import useSWR from 'swr'
import { useInterval } from '../modules/useInterval';

const fetcher = async (url) => await axios.get(url).then((res) => res.data);

export function Timelapse({ }) {

    const [selected, setSelected] = useState(0);
    const [playing, setPlaying] = useState(0);

    const vx = useSWR('/api/videos', fetcher);

    const videos = vx.data;



    var vc = []

    var i = 0;



    useInterval(async () => {
        const { data } = await axios.get('/api/state');
        if (data.playing != playing)
            setPlaying(data.playing);
        if (data.selected != selected)
            setSelected(data.selected);
    }, 100);

    if (!videos)
        return (<h1>Loading...</h1>)

    for (var video of videos) {


        var x_of_selected = selected % 3
        var y_of_selected = (selected - (selected % 3)) / 3

        var starting_id = y_of_selected * 3;

        if (i - starting_id >= 6)
            break

        if (starting_id <= i)
            vc.push(
                <VideoContainer thumbnail={video.thumbnail} name={video.name} selected={(selected == i && playing != i)} playing={playing == i} />
            )
        i++;
    }



    return (<div className={style.cont}>
        <h1 className={style.np}>NOW PLAYING: <b className={style.KILLME}>{videos[playing].name}</b></h1>
        {
            vc
        }
    </div>)
}
export function VideoContainer({
    thumbnail,
    selected,
    name,
    playing
}) {
    return (
        <div className={style.v_c} style={{ backgroundImage: `url("/thumbnails/${thumbnail}")` }}>
            {
                selected ? (
                    <div className={style.selected_c}>
                        <span className={style.v_t}>{name}</span>
                    </div>
                ) :
                    ``
            }
            {
                playing ? (
                    <div className={style.selected_w}>
                        <span className={style.pb} />
                    </div>
                ) :
                    ``
            }
        </div>
    )
}