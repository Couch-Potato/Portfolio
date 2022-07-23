import videos from './videos.json'
export default function handler(req, res) {
    res.status(200).json(videos);
  }
  