import * as yup from "yup";

const schema = yup.object().shape({
  userName: yup.string().min(6).required(),
  password: yup.string().min(6).required(),
});

export default schema;
