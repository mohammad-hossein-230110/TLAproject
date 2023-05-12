using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

#region auxiliary_classes
public class FA
{
    public string states { get; set; }
    public string input_symbols { get; set; }
    public Dictionary<string, Dictionary<string, string>> transitions { get; set; }
    public string initial_state { get; set; }
    public string final_states { get; set; }

}
public class mystate
{
    public string Name { get; set; }
    public Dictionary<string, List<mystate>> next_state;

    public mystate(string name)
    {
        Name = name;
        next_state = new Dictionary<string, List<mystate>>();
    }

    public void add_neighbor(string str, mystate sta)
    {
        if (next_state.ContainsKey(str))
        {
            next_state[str].Add(sta);
        }
        else
        {
            next_state.Add(str, new List<mystate>());
            add_neighbor(str, sta);
        }
    }
}
public class toJson
{
    public string states { get; set; }
    public string input_symbols { get; set; }
    public Dictionary<string, Dictionary<string, string>> transitions { get; set; }
    public string initial_state { get; set; }
    public string final_states { get; set; }

    public toJson(List<string> Alphabet,
    Dictionary<string, Dictionary<string, string>> tran,
    string initial, List<string> final_state)
    {
        #region func_states
        var sb = new StringBuilder();
        sb.Append("{");
        foreach (var t in tran)
        {
            sb.Append($"'{t.Key}'");
            sb.Append(",");
        }
        sb.Remove(sb.Length - 1, 1);
        sb.Append("}");
        #endregion
        states = sb.ToString();

        var Alphbet_String = new StringBuilder();
        Alphbet_String.Append("{");
        foreach (var alphabet in Alphabet)
        {
            Alphbet_String.Append($"'{alphabet}'");
            Alphbet_String.Append(",");
        }
        Alphbet_String.Remove(Alphbet_String.Length - 1, 1);
        Alphbet_String.Append("}");

        input_symbols = Alphbet_String.ToString();

        transitions = tran;
        initial_state = initial;
        #region func_final
        sb = new StringBuilder();
        sb.Append("{");
        foreach (var t in final_state)
        {
            sb.Append($"'{t}',");
        }
        sb.Remove(sb.Length - 1, 1);
        sb.Append("}");
        #endregion
        final_states = sb.ToString();
    }
}
#endregion

class Operation
{
    #region main
    public static void Main()
    {
        string Get_Input = "1.Star operation\r\n2.Concat operation\r\n3.Union operation\r\nPlease Enter the Number of your choise : ";

        Console.WriteLine(Get_Input);

        int InputNumber = int.Parse(Console.ReadLine());

        string JsonFile1 = "C:\\Users\\Mohamad hossein\\TLA01-Projects\\samples\\phase4-sample\\concat\\in\\FA1.json";


        var execute1 = initial(JsonFile1);

        bool IsDfa1 = !execute1.transitions.ToList()[0].Value.ToList()[0].Value.ToString().Contains("{");

        var InputFa1 = analyze(execute1, IsDfa1);

        if (InputFa1.Item3.Count > 1)
        {
            Add_Single_Final_State(ref InputFa1);
        }

        if (InputNumber == 1)
        {
            InputFa1 = Star_Operation( InputFa1);


            List<string> Final_State_Name = InputFa1.Item3.Select(x => x.Name).ToList();


            var nfa_dict = new Dictionary<string, Dictionary<string, string>>();
            foreach (var s in InputFa1.Item1)
            {
                var iter_dict = s.next_state.ToList();
                var res_dict = new Dictionary<string, string>();
                foreach (var i in iter_dict)
                {
                    var sb = new StringBuilder();
                    sb.Append("{");
                    foreach (var str in i.Value)
                    {
                        sb.Append($"'{str.Name}' ,");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append("}");
                    if (i.Key == "landa")
                        res_dict.Add("", sb.ToString());
                    else
                        res_dict.Add(i.Key, sb.ToString());
                }
                nfa_dict.Add(s.Name, res_dict);
            }

            var finish = new toJson(InputFa1.Item2, nfa_dict, InputFa1.Item4.Name, Final_State_Name);
            string json = JsonSerializer.Serialize(finish);
            File.WriteAllText("myout_2.json", json);
        }
        else if (InputNumber == 2)
        {

            string JsonFile2 = "C:\\Users\\Mohamad hossein\\TLA01-Projects\\samples\\phase4-sample\\concat\\in\\FA2.json";

            var execute2 = initial(JsonFile2);
            bool IsDfa2 = !execute1.transitions.ToList()[0].Value.ToList()[0].Value.ToString().Contains("{");

            var InputFa2 = analyze(execute2, IsDfa2);

            if (InputFa2.Item3.Count > 1)
            {
                Add_Single_Final_State(ref InputFa2);
            }

            var Concat_Nfa = Concat_Operation(InputFa1, InputFa2);

            List<string> Final_State_Name = Concat_Nfa.Item3.Select(x => x.Name).ToList();

            var nfa_dict = new Dictionary<string, Dictionary<string, string>>();
            foreach (var s in Concat_Nfa.Item1)
            {
                var iter_dict = s.next_state.ToList();
                var res_dict = new Dictionary<string, string>();
                foreach (var i in iter_dict)
                {
                    var sb = new StringBuilder();
                    sb.Append("{");
                    foreach (var str in i.Value)
                    {
                        sb.Append($"'{str.Name}' ,");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append("}");
                    if (i.Key == "landa")
                        res_dict.Add("", sb.ToString());
                    else
                        res_dict.Add(i.Key, sb.ToString());
                }
                nfa_dict.Add(s.Name, res_dict);
            }

            var finish = new toJson(Concat_Nfa.Item2, nfa_dict, Concat_Nfa.Item4.Name, Final_State_Name);
            string json = JsonSerializer.Serialize(finish);
            File.WriteAllText("myout_2.json", json);

        }
        else if (InputNumber == 3)
        {

            string JsonFile2 = "C:\\Users\\Mohamad hossein\\TLA01-Projects\\samples\\phase4-sample\\concat\\in\\FA2.json";

            var execute2 = initial(JsonFile2);
            bool IsDfa2 = !execute1.transitions.ToList()[0].Value.ToList()[0].Value.ToString().Contains("{");

            var InputFa2 = analyze(execute2, IsDfa2);

            if (InputFa2.Item3.Count > 1)
            {
                Add_Single_Final_State(ref InputFa2);
            }

            var Union_Nfa = Union_Operation(InputFa1, InputFa2);

            List<string> Final_State_Name = Union_Nfa.Item3.Select(x => x.Name).ToList();

         
            var nfa_dict = new Dictionary<string, Dictionary<string, string>>();
            foreach (var s in Union_Nfa.Item1)
            {
                var iter_dict = s.next_state.ToList();
                var res_dict = new Dictionary<string, string>();
                foreach (var i in iter_dict)
                {
                    var sb = new StringBuilder();
                    sb.Append("{");
                    foreach (var str in i.Value)
                    {
                        sb.Append($"'{str.Name}' ,");
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append("}");
                    if (i.Key == "landa")
                        res_dict.Add("", sb.ToString());
                    else
                        res_dict.Add(i.Key, sb.ToString());
                }
                nfa_dict.Add(s.Name, res_dict);
            }

            var finish = new toJson(Union_Nfa.Item2, nfa_dict, Union_Nfa.Item4.Name, Final_State_Name);
            string json = JsonSerializer.Serialize(finish);
            File.WriteAllText("myout_2.json", json);
        }

    }
    #endregion

    #region auxiliary fuctions(check_final_state & myname)
    private static bool check_final_state(List<mystate> list_next_theta,
    List<mystate> final)
    {
        bool res = false;
        foreach (var f in final)
        {
            if (list_next_theta.Contains(f))
            {
                res = true;
                return res;
            }
        }
        return res;
    }

    private static string myname(List<mystate> list_next_theta)
    {
        if (list_next_theta.Count == 0)
        {
            return "TRAP";
        }
        var sb = new StringBuilder();
        var llist = list_next_theta.OrderBy(x => Convert.ToInt32(x.Name.Last()));
        foreach (var l in llist)
        {
            sb.Append(l.Name);
        }
        return sb.ToString();
    }
    #endregion

    #region auxiliary functions(mystr_1_2_3 & landatrans)
    private static List<string> mystr_1(string str)
    {
        var sb = new StringBuilder();
        sb.Append(str);
        sb.Remove(str.Length - 1, 1);
        sb.Remove(0, 1);
        return sb.ToString().Split(",").Select(x => $"{x[1]}{x[2]}").ToList();
    }
    private static List<string> mystr_2(string str)
    {
        var sb = new StringBuilder();
        sb.Append(str);
        sb.Remove(str.Length - 1, 1);
        sb.Remove(0, 1);
        return sb.ToString().Split(",").Select(x => $"{x[1]}").ToList();
    }
    private static mystate[] mystr_3(Dictionary<string, mystate> all,
    Dictionary<string, Dictionary<string, string>> transitions, bool IsDfa)
    {
        mystate[] res = new mystate[all.Count];
        var iter = transitions.ToList();
        for (int i = 0; i < iter.Count; i++)
        {
            var inner_dict = iter[i].Value.ToList();
            for (int j = 0; j < inner_dict.Count; j++)
            {

                var trans = inner_dict[j].Key;

                if (!IsDfa)
                {


                    if (trans == "")
                    {
                        trans = "landa";
                    }
                    var state_list = mystr_1(inner_dict[j].Value);
                    for (int k = 0; k < state_list.Count; k++)
                    {
                        all[iter[i].Key].add_neighbor(trans, all[state_list[k]]);
                    }
                }
                else
                {
                    var state_str = inner_dict[j].Value;
                    all[iter[i].Key].add_neighbor(trans, all[state_str]);
                }
            }
            res[i] = all[iter[i].Key];
        }
        return res;
    }
    #endregion

    #region auxiliary fuctions(landatrans, combine)
    private static mystate[] landatrans(mystate[] state)
    {
        var nstate = state.ToList();
        for (int i = 0; i < nstate.Count; i++)
        {
            foreach (var t in nstate[i].next_state)
            {
                if (t.Key == "landa")
                {
                    foreach (var v in t.Value)
                    {
                        combine(nstate[i], v);
                        nstate[i].next_state.Remove("landa");
                        // if (nstate.IndexOf(v) < i)
                        // {
                        //     i--;
                        // }
                        // nstate.Remove(v);
                        // foreach (var n in nstate)
                        // {
                        //     foreach (var val in n.next_state.Values)
                        //     {
                        //         val.Remove();
                        //         val.Add(nstate[i]);
                        //     }
                        // }
                    }
                }
            }
        }
        return nstate.ToArray();

    }

    private static void combine(mystate state_1, mystate state_2)
    {
        var sb = new StringBuilder();
        if (Convert.ToInt32(state_1.Name.Last()) > Convert.ToInt32(state_2.Name.Last()))
        {
            sb.Append(state_2.Name);
            sb.Append(state_1.Name);
        }
        else
        {
            sb.Append(state_1.Name);
            sb.Append(state_2.Name);
        }
        state_1.Name = sb.ToString();
        foreach (var s2 in state_2.next_state)
        {
            if (!state_1.next_state.ContainsKey(s2.Key))
            {
                state_1.next_state.Add(s2.Key, s2.Value);
            }
            else
            {
                foreach (var a in s2.Value)
                {
                    if (!state_1.next_state[s2.Key].Contains(a))
                    {
                        state_1.next_state[s2.Key].Add(a);
                    }
                }
            }
        }
    }
    #endregion

    #region initial&analyze functions

    private static FA initial(string JsonFile)
    {
        string text =
        File.ReadAllText(JsonFile);
        FA test = JsonSerializer.Deserialize<FA>(text);
        return test;
    }

    private static (mystate[], List<string>, List<mystate>, mystate)
    analyze(FA execute, bool IsDfa)
    {
        Dictionary<string, mystate> all = new Dictionary<string, mystate>();
        List<mystate> final_state = new List<mystate>();
        mystate start;
        List<string> n_states = mystr_1(execute.states);
        List<string> n_final = mystr_1(execute.final_states);
        List<string> n_symbols = mystr_2(execute.input_symbols);
        for (int i = 0; i < n_states.Count; i++)
        {
            all.Add(n_states[i], new mystate(n_states[i]));
        }
        for (int i = 0; i < n_final.Count; i++)
        {
            final_state.Add(all[n_final[i]]);
        }
        start = all[execute.initial_state];
        mystate[] res = mystr_3(all, execute.transitions, IsDfa);
        return (res, n_symbols, final_state, start);
    }
    #endregion

    #region Star function & Concat function & Union function & auxiliary function Add_Single_Final_State   

    private static void Add_Single_Final_State(ref (mystate[], List<string>, List<mystate>, mystate) InputFa)
    {
        mystate Single_Final = new mystate("q" + InputFa.Item1.ToList().Count);

        mystate[] With_Single_Final = new mystate[InputFa.Item1.Length + 1];
        for (int i = 0; i < With_Single_Final.Length - 1; i++)
        {
            With_Single_Final[i] = InputFa.Item1[i];
        }
        With_Single_Final[With_Single_Final.Length - 1] = Single_Final;

        for (int i = 0; i < InputFa.Item3.Count; i++)
        {

            InputFa.Item3[i].add_neighbor("landa", Single_Final);

        }

        InputFa.Item3.Clear();
        InputFa.Item3.Add(Single_Final);
        InputFa.Item1 = With_Single_Final;
    }

    private static (mystate[], List<string>, List<mystate>, mystate) Star_Operation( (mystate[], List<string>, List<mystate>, mystate) InputFa)
    {

        mystate Initial_State = new mystate("q" + InputFa.Item1.ToList().Count.ToString());
        mystate Final_State = new mystate("q" + (InputFa.Item1.ToList().Count + 1).ToString());

        Initial_State.add_neighbor("landa", InputFa.Item4);
        Initial_State.add_neighbor("landa", Final_State);
        InputFa.Item3[0].add_neighbor("landa", Final_State);
        Final_State.add_neighbor("landa", Initial_State);


        mystate[] New_Nfa = new mystate[InputFa.Item1.Length + 2];
        for (int i = 0; i < New_Nfa.Length - 2; i++)
        {
            New_Nfa[i] = InputFa.Item1[i];
        }

        New_Nfa[New_Nfa.Length - 2] = Initial_State;
        New_Nfa[New_Nfa.Length - 1] = Final_State;

        InputFa.Item3.Clear();
        InputFa.Item3.Add(Final_State);

        InputFa.Item4 = Initial_State;

        return (New_Nfa, InputFa.Item2, InputFa.Item3, InputFa.Item4);

    }

    private static (mystate[], List<string>, List<mystate>, mystate)
    Union_Operation((mystate[], List<string>, List<mystate>, mystate) InputFa1, (mystate[], List<string>, List<mystate>, mystate) InputFa2)
    {
        mystate Initial_State = new mystate("q" + (InputFa1.Item1.ToList().Count + InputFa2.Item1.ToList().Count).ToString());
        mystate Final_State = new mystate("q" + (InputFa1.Item1.ToList().Count + InputFa2.Item1.ToList().Count + 1).ToString());

        Initial_State.add_neighbor("landa", InputFa1.Item4);
        Initial_State.add_neighbor("landa", InputFa2.Item4);

        InputFa1.Item3[0].add_neighbor("landa", Final_State);
        InputFa2.Item3[0].add_neighbor("landa", Final_State);

        int Count_Of_States = InputFa1.Item1.ToList().Count + InputFa2.Item1.ToList().Count + 2;

        List<mystate> Final_State_List = new List<mystate> { Final_State };

        List<string> Alphabet = InputFa1.Item2.Concat(InputFa2.Item2).ToList();

        mystate[] Union_Nfa_States_Array = new mystate[Count_Of_States];

        int iter = 0;

        for (; iter < InputFa1.Item1.ToList().Count; iter++)
        {
            Union_Nfa_States_Array[iter] = InputFa1.Item1.ToList()[iter];
        }



        for (int i = 0; i < InputFa2.Item1.ToList().Count; iter++, i++)
        {
            InputFa2.Item1.ToList()[i].Name = "q" + iter.ToString();

            Union_Nfa_States_Array[iter] = InputFa2.Item1.ToList()[i];
        }

        Union_Nfa_States_Array[iter] = Initial_State;
        Union_Nfa_States_Array[iter + 1] = Final_State;

        return (Union_Nfa_States_Array, Alphabet, Final_State_List, Initial_State);
    }

    private static (mystate[], List<string>, List<mystate>, mystate) Concat_Operation((mystate[], List<string>, List<mystate>, mystate) InputFa1, (mystate[], List<string>, List<mystate>, mystate) InputFa2)
    {
        mystate Final_State = new mystate("q" + (InputFa1.Item1.ToList().Count + InputFa2.Item1.ToList().Count).ToString());

        InputFa1.Item3[0].add_neighbor("landa", InputFa2.Item4);

        InputFa2.Item3[0].add_neighbor("landa", Final_State);

        mystate Initial_state = InputFa1.Item4;

        List<string> Alphabet = InputFa1.Item2.Concat(InputFa2.Item2).ToList();

        List<mystate> Final_state_List = new List<mystate> { Final_State };

        int Count_Of_States = InputFa1.Item1.ToList().Count + InputFa2.Item1.ToList().Count + 1;

        mystate[] Concat_Nfa_States_Array = new mystate[Count_Of_States];

        int iter = 0;

        for (; iter < InputFa1.Item1.ToList().Count; iter++)
        {
            Concat_Nfa_States_Array[iter] = InputFa1.Item1.ToList()[iter];
        }

        for (int i = 0; i < InputFa2.Item1.ToList().Count; iter++, i++)
        {
            InputFa2.Item1.ToList()[i].Name = "q" + iter.ToString();

            Concat_Nfa_States_Array[iter] = InputFa2.Item1.ToList()[i];
        }

        Concat_Nfa_States_Array[iter] = Final_State;


        return (Concat_Nfa_States_Array, Alphabet, Final_state_List, Initial_state);
    }

    #endregion

}